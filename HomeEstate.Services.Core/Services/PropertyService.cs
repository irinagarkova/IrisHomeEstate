using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Data.Models.Enum;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
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

            // Запазване на снимките ако има такива
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

        public async Task<ICollection<PropertyDto>> GetAllPropertiesAsync()
        {
            var properties = await dbContext.Properties
                .Include(p => p.Location)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            var mappedProperties = properties.Select(p => mapper.Map<PropertyDto>(p)).ToList();
            return mappedProperties;
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

            mapper.Map(property, propertyToUpdate);

            // Обновяване на снимки ако има нови
            if (property.Images != null && property.Images.Any())
            {
                // Премахване на стари снимки
                var oldImages = await dbContext.PropertyImages
                    .Where(pi => pi.PropertyId == property.Id)
                    .ToListAsync();

                dbContext.PropertyImages.RemoveRange(oldImages);

                // Добавяне на нови снимки
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
        public async Task<ICollection<PropertyDto>> SearchPropertiesAsync(PropertySearchDto searchCriteria)
        {
            Console.WriteLine($"SearchPropertiesAsync called with ListingType: {searchCriteria.ListingType}");

            var query = dbContext.Properties
                .Include(p => p.Location)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.FavoriteProperties)
                .Where(p => !p.IsDeleted);

            // DEBUG: Логирайте колко properties има общо
            var totalCount = await query.CountAsync();
            Console.WriteLine($"Total properties in DB: {totalCount}");

            // Apply filters
            if (!string.IsNullOrEmpty(searchCriteria.Location))
            {
                query = query.Where(p => p.Location.City.Contains(searchCriteria.Location) ||
                                        p.Location.Address.Contains(searchCriteria.Location));
                Console.WriteLine($"After location filter: {await query.CountAsync()} properties");
            }

            if (searchCriteria.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchCriteria.CategoryId.Value);
                Console.WriteLine($"After category filter: {await query.CountAsync()} properties");
            }

            // ПОПРАВЕНА ЛОГИКА ЗА LISTING TYPE
            if (searchCriteria.ListingType.HasValue)
            {
                query = query.Where(p => p.ListingType == searchCriteria.ListingType.Value ||
                                        p.ListingType == PropertyListingType.Both);
                Console.WriteLine($"After listing type filter: {await query.CountAsync()} properties");
            }

            // ПОПРАВЕНА ЛОГИКА ЗА PRICE/RENT FILTERS
            // Ако търсим за Sale properties или не е указан тип
            if (searchCriteria.ListingType == PropertyListingType.Sale ||
                (!searchCriteria.ListingType.HasValue && searchCriteria.MaxPrice.HasValue))
            {
                if (searchCriteria.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= searchCriteria.MinPrice.Value);
                }

                if (searchCriteria.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= searchCriteria.MaxPrice.Value);
                }
                Console.WriteLine($"After sale price filter: {await query.CountAsync()} properties");
            }

            // Ако търсим за Rent properties
            if (searchCriteria.ListingType == PropertyListingType.Rent ||
                searchCriteria.ListingType == PropertyListingType.Both ||
                (!searchCriteria.ListingType.HasValue && searchCriteria.MaxRent.HasValue))
            {
                if (searchCriteria.MinRent.HasValue)
                {
                    query = query.Where(p => p.MonthlyRent.HasValue && p.MonthlyRent >= searchCriteria.MinRent.Value);
                }

                if (searchCriteria.MaxRent.HasValue)
                {
                    query = query.Where(p => p.MonthlyRent.HasValue && p.MonthlyRent <= searchCriteria.MaxRent.Value);
                }

                if (searchCriteria.PetsAllowed.HasValue)
                {
                    query = query.Where(p => p.PetsAllowed == searchCriteria.PetsAllowed.Value);
                }

                if (searchCriteria.IsFurnished.HasValue)
                {
                    query = query.Where(p => p.IsFurnished == searchCriteria.IsFurnished.Value);
                }
                Console.WriteLine($"After rent filters: {await query.CountAsync()} properties");
            }

            // Common filters
            if (searchCriteria.MinArea.HasValue)
            {
                query = query.Where(p => p.Area >= searchCriteria.MinArea.Value);
            }

            if (searchCriteria.MaxArea.HasValue)
            {
                query = query.Where(p => p.Area <= searchCriteria.MaxArea.Value);
            }

            // Apply sorting
            query = searchCriteria.SortBy?.ToLower() switch
            {
                "price-asc" => query.OrderBy(p => p.ListingType == PropertyListingType.Rent ? p.MonthlyRent : p.Price),
                "price-desc" => query.OrderByDescending(p => p.ListingType == PropertyListingType.Rent ? p.MonthlyRent : p.Price),
                "date-asc" => query.OrderBy(p => p.CreatedOn),
                "date-desc" => query.OrderByDescending(p => p.CreatedOn),
                "area-asc" => query.OrderBy(p => p.Area),
                "area-desc" => query.OrderByDescending(p => p.Area),
                _ => query.OrderByDescending(p => p.CreatedOn)
            };

            var properties = await query.ToListAsync();
            Console.WriteLine($"Final result: {properties.Count} properties");

            var result = properties.Select(p =>
            {
                var dto = mapper.Map<PropertyDto>(p);
                dto.FavoriteCount = p.FavoriteProperties.Count;
                return dto;
            }).ToList();

            return result;
        }

        public async Task<PropertyStatisticsDto> GetUserPropertyStatisticsAsync(string userId)
        {
            var userProperties = await dbContext.Properties
         .Where(p => p.OwnerId == userId && !p.IsDeleted)
         .Include(p => p.FavoriteProperties) // Включете favorites
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

		public IEnumerable<LocationDto> GetAllLocations()
		{
			return dbContext.Locations
					.Select(l => new LocationDto
					{
						Id = l.Id,
						City = l.City,
						Address = l.Address
					})
					.OrderBy(l => l.City)
					.ToList();
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

        // Нов метод за получаване на rental статистики
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
    }
}