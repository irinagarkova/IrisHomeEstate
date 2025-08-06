using AutoMapper;
using Azure;
using HomeEstate.Data;
using HomeEstate.Data.Models.Enum;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly HomeEstateDbContext dbContext;
        private readonly IMapper mapper;

        public PropertyService(HomeEstateDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task CreatePropertyAsync(PropertyDto property)
        {
            // Валидация за rental свойства
            ValidateRentalProperties(property);

            var newProperty = mapper.Map<Property>(property);
            newProperty.CreatedOn = DateTime.UtcNow;

            await dbContext.AddAsync(newProperty);
            await dbContext.SaveChangesAsync();

            if (property.Images != null && property.Images.Any())
            {
                var propertyImages = property.Images.Select(img => new PropertyImage
                {
                    PropertyId = newProperty.Id,
                    ImageUrl = img.ImageUrl,
                    IsDeleted = false
                }).ToList();

                await dbContext.PropertyImages.AddRangeAsync(propertyImages);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeletePropertyAsync(int id)
        {
            var property = await getProperty(id);
            if (property != null)
            {
                property.IsDeleted = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<Pagination<PropertyDto>> GetAllPropertiesAsync(int page, int pageSize)
        {
            // Validate pagination parameters
            page = Math.Max(1, page); // Ensure page is at least 1
            pageSize = Math.Min(Math.Max(1, pageSize), 100); // Ensure pageSize is between 1 and 100

            // Get total count for pagination info
            var totalProperties = await GetTotalPropertiesCount();

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalProperties / pageSize);

            var properties = await dbContext.Properties
                .Include(p => p.Location)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            var mappedProps = mapper.Map<List<PropertyDto>>(properties);
            return new Pagination<PropertyDto>
            {
                Items = mappedProps,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalProperties,
                TotalPages = totalPages
            };

        }

        public async Task<PropertyDto> GetPropertyAsync(int id)
        {
            var property = await dbContext.Properties
                .Include(p => p.Location)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (property == null)
            {
                throw new NotFoundException("Property", id);
            }

            var mappedProp = mapper.Map<PropertyDto>(property);
            return mappedProp;
        }

        private async Task<Property> getProperty(int id)
        {
            var property = await dbContext.Properties
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                throw new NotFoundException("Property", id);
            }

            return property;
        }

        public async Task UpdatePropertyAsync(PropertyDto property)
        {
            var errors = new Dictionary<string, string>();

            // Съществуваща валидация
            if (property.Area <= 0)
            {
                errors.Add("Area", "Area must be at least 1");
            }

            if (property.Title.Length < 5 || property.Title.Length > 100)
            {
                errors.Add("Title", "Title must be between 5 and 100 characters");
            }

            // Нова валидация за rental свойства
            ValidateRentalProperties(property, errors);

            if (errors.Any())
            {
                throw new CustomValidationException("Validation failed", errors);
            }

            var propertyToUpdate = await dbContext.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == property.Id);

            if (propertyToUpdate == null)
            {
                throw new NotFoundException("Property", property.Id);
            }

            //mapper.Map(property, propertyToUpdate);
            propertyToUpdate.LocationId = property.LocationId;
            propertyToUpdate.Price = property.Price;
            propertyToUpdate.CreatedOn = property.CreatedOn;
            propertyToUpdate.AvailableFrom = property.AvailableFrom;
            propertyToUpdate.Area = property.Area;
            propertyToUpdate.CategoryId = property.CategoryId;
            propertyToUpdate.ListingType = property.ListingType;
            propertyToUpdate.PropertyType = property.PropertyType;

            // Обновяване на снимки
            if (property.Images != null && property.Images.Any())
            {
                // Премахване на стари снимки
                var oldImages = await dbContext.PropertyImages
                    .Where(pi => pi.PropertyId == property.Id)
                    .ToListAsync();

                dbContext.PropertyImages.RemoveRange(oldImages);

         
                var newImages = property.Images.Select(img => new PropertyImage
                {
                    PropertyId = property.Id,
                    ImageUrl = img.ImageUrl,
                    IsDeleted = false
                }).ToList();

                await dbContext.PropertyImages.AddRangeAsync(newImages);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<PropertyDto>> GetPropertiesByUserIdAsync(string userId)
        {
            var properties = await dbContext.Properties
                .Where(p => p.OwnerId == userId && !p.IsDeleted)
                .Include(p => p.Images)
                .Include(p => p.Location)
                .Include(p => p.Category)
                .Include(p => p.FavoriteProperties)
                .ToListAsync();

            var propertyDtos = properties.Select(p =>
            {
                var dto = mapper.Map<PropertyDto>(p);
                dto.FavoriteCount = p.FavoriteProperties.Count; 
                return dto;
            }).ToList();

            return propertyDtos;
        }

        // New methods for rent/sale functionality
        public async Task<ICollection<PropertyDto>> GetPropertiesByTypeAsync(PropertyListingType type)
        {
            var properties = await dbContext.Properties
                .Include(p => p.Location)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted && (p.ListingType == type || p.ListingType == PropertyListingType.Both))
                .ToListAsync();

            return mapper.Map<ICollection<PropertyDto>>(properties);
        }

        public async Task<ICollection<PropertyDto>> GetPropertiesForSaleAsync()
        {
            return await GetPropertiesByTypeAsync(PropertyListingType.Sale);
        }

        public async Task<ICollection<PropertyDto>> GetPropertiesForRentAsync()
        {
            return await GetPropertiesByTypeAsync(PropertyListingType.Rent);
        }
        public async Task<Pagination<PropertyDto>> SearchPropertiesAsync(PropertySearchDto searchCriteria)
        {
            var query = dbContext.Properties
       .Include(p => p.Location)
       .Include(p => p.Category)
       .Include(p => p.Images)
       .Include(p => p.FavoriteProperties)
       .Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(searchCriteria.Location))
            {
                var locationLower = searchCriteria.Location.ToLower();
                query = query.Where(p =>
                    p.Location.City.ToLower().Contains(locationLower) ||
                    p.Location.Address.ToLower().Contains(locationLower));
            }

            if (searchCriteria.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchCriteria.CategoryId.Value);
            }

            if (searchCriteria.ListingType.HasValue)
            {
                query = query.Where(p =>
                    p.ListingType == searchCriteria.ListingType.Value ||
                    p.ListingType == PropertyListingType.Both);
            }

            if (searchCriteria.ListingType == PropertyListingType.Sale || !searchCriteria.ListingType.HasValue)
            {
                if (searchCriteria.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= searchCriteria.MinPrice.Value);
                }

                if (searchCriteria.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= searchCriteria.MaxPrice.Value);
                }
            }

            if (searchCriteria.ListingType == PropertyListingType.Rent ||
                searchCriteria.ListingType == PropertyListingType.Both ||
                searchCriteria.MinRent.HasValue ||
                searchCriteria.MaxRent.HasValue)
            {
                if (searchCriteria.MinRent.HasValue)
                {
                    query = query.Where(p => p.MonthlyRent.HasValue && p.MonthlyRent >= searchCriteria.MinRent.Value);
                }

                if (searchCriteria.MaxRent.HasValue)
                {
                    query = query.Where(p => p.MonthlyRent.HasValue && p.MonthlyRent <= searchCriteria.MaxRent.Value);
                }
            }

            if (searchCriteria.MinArea.HasValue)
            {
                query = query.Where(p => p.Area >= searchCriteria.MinArea.Value);
            }

            if (searchCriteria.MaxArea.HasValue)
            {
                query = query.Where(p => p.Area <= searchCriteria.MaxArea.Value);
            }

            if (searchCriteria.Bedrooms.HasValue)
            {
                var propertyType = (PropertyType)searchCriteria.Bedrooms.Value;
                query = query.Where(p => p.PropertyType == propertyType);
            }

            if (searchCriteria.PetsAllowed.HasValue)
            {
                query = query.Where(p => p.PetsAllowed == searchCriteria.PetsAllowed.Value);
            }

            if (searchCriteria.IsFurnished.HasValue)
            {
                query = query.Where(p => p.IsFurnished == searchCriteria.IsFurnished.Value);
            }

            // Sorting
            query = searchCriteria.SortBy?.ToLower() switch
            {
                "price-asc" => query.OrderBy(p => p.ListingType == PropertyListingType.Rent ? p.MonthlyRent ?? p.Price : p.Price),
                "price-desc" => query.OrderByDescending(p => p.ListingType == PropertyListingType.Rent ? p.MonthlyRent ?? p.Price : p.Price),
                "date-asc" => query.OrderBy(p => p.CreatedOn),
                "date-desc" => query.OrderByDescending(p => p.CreatedOn),
                "newest" => query.OrderByDescending(p => p.CreatedOn),
                "area-asc" => query.OrderBy(p => p.Area),
                "area-desc" => query.OrderByDescending(p => p.Area),
                _ => query.OrderByDescending(p => p.CreatedOn) 
            };

            var properties = await query.ToListAsync();

            var result = properties
                .OrderBy(x=>x.Price)
                .Skip((searchCriteria.Page -1) * searchCriteria.PageSize)
                .Take(searchCriteria.PageSize)
                .Select(p =>
            {
                var dto = mapper.Map<PropertyDto>(p);
                dto.FavoriteCount = p.FavoriteProperties.Count;
                return dto;
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)properties.Count / searchCriteria.PageSize);

            return new Pagination<PropertyDto>
            {
                Items = result,
                CurrentPage = searchCriteria.Page,
                TotalPages = totalPages,
                PageSize = result.Count,
                TotalItems = result.Count
            };

        }

        public async Task<PropertyStatisticsDto> GetUserPropertyStatisticsAsync(string userId)
        {
            var userProperties = await dbContext.Properties
         .Where(p => p.OwnerId == userId && !p.IsDeleted)
         .Include(p => p.FavoriteProperties)
         .ToListAsync();

            var statistics = new PropertyStatisticsDto
            {
                TotalProperties = userProperties.Count,
                PropertiesForSale = userProperties.Count(p => p.ListingType == PropertyListingType.Sale || p.ListingType == PropertyListingType.Both),
                PropertiesForRent = userProperties.Count(p => p.ListingType == PropertyListingType.Rent || p.ListingType == PropertyListingType.Both),
                ActiveListings = userProperties.Count(p => !p.IsDeleted),
                TotalValue = userProperties.Where(p => p.ListingType != PropertyListingType.Rent).Sum(p => p.Price),
                AveragePrice = userProperties.Where(p => p.ListingType != PropertyListingType.Rent).Any()
                    ? userProperties.Where(p => p.ListingType != PropertyListingType.Rent).Average(p => p.Price)
                    : 0,
                AverageRent = userProperties.Where(p => p.MonthlyRent.HasValue).Any()
                    ? userProperties.Where(p => p.MonthlyRent.HasValue).Average(p => p.MonthlyRent.Value)
                    : 0,
                TotalViews = 0, 
                TotalFavorites = userProperties.Sum(p => p.FavoriteProperties.Count)
            };

            return statistics;
        }

        public async Task<ICollection<PropertyDto>> GetAllPropertiesAsync(SearchPropertyDto search)
        {
            var allprop = await dbContext.Properties
                 .Where(p => p.Price < search.MaxPrice && p.Location.City.ToLower().Contains(search.Location.ToLower())
                 && p.CategoryId == search.CategoryId)
                 .Include(p => p.Location)
                 .Include(p => p.Category)
                 .Include(p => p.Images)
                 .ToListAsync();

            var mapped = allprop.Select(ap => mapper.Map<PropertyDto>(ap)).ToList();
            return mapped;


        }

		public async Task<IEnumerable<LocationDto>> GetAllLocations()
		{
			return await dbContext.Locations
					.Select(l => new LocationDto
					{
						Id = l.Id,
						City = l.City,
						Address = l.Address
					})
					.OrderBy(l => l.City)
					.ToListAsync();
		}
        private void ValidateRentalProperties(PropertyDto property, Dictionary<string, string> errors = null)
        {
            if (errors == null)
                errors = new Dictionary<string, string>();

            if (property.ListingType == PropertyListingType.Rent || property.ListingType == PropertyListingType.Both)
            {
                if (!property.MonthlyRent.HasValue || property.MonthlyRent <= 0)
                {
                    errors.Add("MonthlyRent", "Monthly rent is required for rental properties");
                }

                if (property.MinimumLeasePeriod.HasValue && (property.MinimumLeasePeriod < 1 || property.MinimumLeasePeriod > 60))
                {
                    errors.Add("MinimumLeasePeriod", "Minimum lease period must be between 1 and 60 months");
                }

                if (property.SecurityDeposit.HasValue && property.SecurityDeposit < 0)
                {
                    errors.Add("SecurityDeposit", "Security deposit cannot be negative");
                }

                if (property.AvailableFrom.HasValue && property.AvailableFrom < DateTime.Today)
                {
                    errors.Add("AvailableFrom", "Available from date cannot be in the past");
                }
            }

            if (errors.Any())
            {
                throw new CustomValidationException("Rental validation failed", errors);
            }
        }

        // rental статистики
        public async Task<RentalStatisticsDto> GetRentalStatisticsAsync(string userId)
        {
            var userProperties = await dbContext.Properties
                .Where(p => p.OwnerId == userId && !p.IsDeleted)
                .ToListAsync();

            var rentalProperties = userProperties.Where(p =>
                p.ListingType == PropertyListingType.Rent ||
                p.ListingType == PropertyListingType.Both).ToList();

            return new RentalStatisticsDto
            {
                TotalRentalProperties = rentalProperties.Count,
                AverageMonthlyRent = rentalProperties.Where(p => p.MonthlyRent.HasValue).Any()
                    ? rentalProperties.Where(p => p.MonthlyRent.HasValue).Average(p => p.MonthlyRent.Value)
                    : 0,
                TotalMonthlyIncome = rentalProperties.Where(p => p.MonthlyRent.HasValue).Sum(p => p.MonthlyRent.Value),
                FurnishedProperties = rentalProperties.Count(p => p.IsFurnished == true),
                PetFriendlyProperties = rentalProperties.Count(p => p.PetsAllowed == true),
                PropertiesWithParking = rentalProperties.Count(p => p.IsParking == true)
            };
        }

        public async Task<int> GetTotalPropertiesCount()
        {
            var count = await dbContext.Properties.CountAsync();
            return count;
        }

        public async Task<List<PropertyDto>> GetRecentProperties(int count)
        {

            var properties = await dbContext.Properties.ToListAsync();
            var mapped = new List<PropertyDto>();
            for (int i = properties.Count - 1; i > properties.Count - count; i--)
            {
                mapped.Add(mapper.Map<PropertyDto>(properties[i]));
            }
            return mapped;
        }

        //public async Task<ICollection<PropertyDto>> GetSimilarPropertiesAsync(int propertyId, int count = 4)
        //{
        //    Първо вземете текущия имот за да знаете категорията и локацията
        //    var currentProperty = await dbContext.Properties
        //        .Include(p => p.Category)
        //        .Include(p => p.Location)
        //        .FirstOrDefaultAsync(p => p.Id == propertyId && !p.IsDeleted);

        //    if (currentProperty == null)
        //        return new List<PropertyDto>();

        //    var similarProperties = await dbContext.Properties
        //        .Include(p => p.Location)
        //        .Include(p => p.Category)
        //        .Include(p => p.Images)
        //        .Include(p => p.FavoriteProperties)
        //        .Where(p => !p.IsDeleted &&
        //                   p.Id != propertyId && // Изключете текущия имот
        //                   (p.CategoryId == currentProperty.CategoryId || // Същата категория
        //                    p.LocationId == currentProperty.LocationId)) // Или същата локация
        //        .OrderBy(p => p.CategoryId == currentProperty.CategoryId ? 0 : 1) // Приоритет на категорията
        //        .ThenBy(p => Math.Abs(p.Price - currentProperty.Price)) // Сортиране по близка цена
        //        .Take(count)
        //        .ToListAsync();

        //    Ако няма достатъчно подобни, добавете други рандом имоти
        //    if (similarProperties.Count < count)
        //    {
        //        var additionalProperties = await dbContext.Properties
        //            .Include(p => p.Location)
        //            .Include(p => p.Category)
        //            .Include(p => p.Images)
        //            .Include(p => p.FavoriteProperties)
        //            .Where(p => !p.IsDeleted &&
        //                       p.Id != propertyId &&
        //                       !similarProperties.Select(sp => sp.Id).Contains(p.Id))
        //            .OrderBy(x => Guid.NewGuid()) // Рандом подредба
        //            .Take(count - similarProperties.Count)
        //            .ToListAsync();

        //        similarProperties.AddRange(additionalProperties);
        //    }

        //    return similarProperties.Select(p => mapper.Map<PropertyDto>(p)).ToList();
        //}

        // Добавете този метод в края на PropertyService класа, преди затварящата скоба

        public async Task<ICollection<PropertyDto>> GetSimilarPropertiesAsync(int propertyId, int count)
        {
            try
            {
                // Вземане на основния имот
                var mainProperty = await dbContext.Properties
                    .Include(p => p.Location)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == propertyId && !p.IsDeleted);

                if (mainProperty == null)
                {
                    return new List<PropertyDto>();
                }

                // Търсене на подобни имоти според критерии:
                // 1. Същия тип (категория)
                // 2. Подобна цена (± 30%)
                // 3. Същия град
                // 4. Не включваме същия имот

                var priceRange = mainProperty.Price * 0.3m; // 30% разлика в цената
                var minPrice = mainProperty.Price - priceRange;
                var maxPrice = mainProperty.Price + priceRange;

                var similarProperties = await dbContext.Properties
                    .Include(p => p.Location)
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .Include(p => p.FavoriteProperties)
                    .Where(p => !p.IsDeleted &&
                               p.Id != propertyId && // Не включваме същия имот
                               (
                                   // Същата категория
                                   p.CategoryId == mainProperty.CategoryId ||
                                   // Същия град
                                   p.LocationId == mainProperty.LocationId ||
                                   // Подобна цена
                                   (p.Price >= minPrice && p.Price <= maxPrice)
                               ))
                    .OrderBy(p =>
                        // Сортиране по приоритет - най-близо до основния имот
                        Math.Abs(p.Price - mainProperty.Price) +
                        (p.CategoryId == mainProperty.CategoryId ? 0 : 1000000) +
                        (p.LocationId == mainProperty.LocationId ? 0 : 500000))
                    .Take(count)
                    .ToListAsync();

                // Ако не намерим достатъчно подобни, добавяме други случайни имоти
                if (similarProperties.Count < count)
                {
                    var additionalProperties = await dbContext.Properties
                        .Include(p => p.Location)
                        .Include(p => p.Category)
                        .Include(p => p.Images)
                        .Include(p => p.FavoriteProperties)
                        .Where(p => !p.IsDeleted &&
                                   p.Id != propertyId &&
                                   !similarProperties.Select(sp => sp.Id).Contains(p.Id))
                        .OrderByDescending(p => p.CreatedOn)
                        .Take(count - similarProperties.Count)
                        .ToListAsync();

                    similarProperties.AddRange(additionalProperties);
                }

                return similarProperties.Select(p => mapper.Map<PropertyDto>(p)).ToList();
            }
            catch (Exception ex)
            {
                // В случай на грешка връщаме празен списък
                return new List<PropertyDto>();
            }
        }

    }
}