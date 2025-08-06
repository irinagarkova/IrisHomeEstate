using AutoMapper;
using FakeItEasy;
using HomeEstate.Data;
using HomeEstate.Data.Models.Enum;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Services;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace HomeEstate.Services.Core.Tests.Services
{
    public class PropertyServiceTests : IDisposable
    {
        private readonly HomeEstateDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly PropertyService _service;

        public PropertyServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<HomeEstateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _dbContext = new HomeEstateDbContext(options);

            // Ensure database is created
            _dbContext.Database.EnsureCreated();

                // Remove seed data
                _dbContext.Properties.RemoveRange(_dbContext.Properties);
                _dbContext.Categories.RemoveRange(_dbContext.Categories);
                _dbContext.Locations.RemoveRange(_dbContext.Locations);
                _dbContext.PropertyImages.RemoveRange(_dbContext.PropertyImages);
                _dbContext.FavoriteProperties.RemoveRange(_dbContext.FavoriteProperties);
                _dbContext.SaveChanges();
            // Setup fake mapper
            _mapper = A.Fake<IMapper>();

            _service = new PropertyService(_dbContext, _mapper);
        }

        [Fact]
        public async Task GetPropertyAsync_WhenPropertyExists_ShouldReturnProperty()
        {
            // Arrange
            var property = await CreateTestPropertyDirectly();
            var expectedDto = CreateTestPropertyDto(property);

            A.CallTo(() => _mapper.Map<PropertyDto>(A<Property>._)).Returns(expectedDto);
            
            // Act
            var result = await _service.GetPropertyAsync(property.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(property.Id);
        }

        [Fact]
        public async Task GetPropertyAsync_WhenPropertyDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = 999;

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(() => _service.GetPropertyAsync(nonExistentId));
            exception.Message.ShouldContain("Property");
            exception.Message.ShouldContain(nonExistentId.ToString());
        }

        [Fact]
        public async Task CreatePropertyAsync_WithValidProperty_ShouldCreateSuccessfully()
        {
            // Arrange
            await SetupTestData();

            var propertyDto = new PropertyDto
            {
                Title = "New Property",
                Description = "Test Description",
                Price = 100000,
                Area = 120,
                OwnerId = "owner-id",
                CategoryId = 1,
                LocationId = 1,
                ListingType = PropertyListingType.Sale,
                Images = new List<PropertyImageDto>
                {
                    new PropertyImageDto { ImageUrl = "test1.jpg" },
                    new PropertyImageDto { ImageUrl = "test2.jpg" }
                }
            };

            var property = new Property
            {
                Title = propertyDto.Title,
                Description = propertyDto.Description,
                Price = propertyDto.Price,
                Area = propertyDto.Area,
                OwnerId = propertyDto.OwnerId,
                CategoryId = propertyDto.CategoryId,
                LocationId = propertyDto.LocationId,
                ListingType = propertyDto.ListingType
            };

            A.CallTo(() => _mapper.Map<Property>(propertyDto)).Returns(property);

            // Act
            await _service.CreatePropertyAsync(propertyDto);

            // Assert
            var createdProperty = await _dbContext.Properties
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Title == propertyDto.Title);
            createdProperty.ShouldNotBeNull();
            createdProperty.Title.ShouldBe(propertyDto.Title);
        }

        [Fact]
        public async Task DeletePropertyAsync_WhenPropertyExists_ShouldSoftDelete()
        {
            // Arrange
            var property = await CreateTestPropertyDirectly();

            // Act
            await _service.DeletePropertyAsync(property.Id);

            // Assert
            var deletedProperty = await _dbContext.Properties
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == property.Id);
            deletedProperty.ShouldNotBeNull();
            deletedProperty.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public async Task GetTotalPropertiesCount_ShouldReturnCorrectCount()
        {
            // Arrange
            await CreateTestPropertyDirectly();
            await CreateTestPropertyDirectly();
            await CreateTestPropertyDirectly();

            // Act
            var result = await _service.GetTotalPropertiesCount();

            // Assert
            result.ShouldBe(3);
        }

        [Fact]
        public async Task GetPropertiesForSaleAsync_ShouldReturnOnlySaleProperties()
        {
            // Arrange
            var saleProperty = await CreateTestPropertyDirectly(listingType: PropertyListingType.Sale);
            var rentProperty = await CreateTestPropertyDirectly(listingType: PropertyListingType.Rent);
            var bothProperty = await CreateTestPropertyDirectly(listingType: PropertyListingType.Both);

            var expectedDtos = new List<PropertyDto>
            {
                new PropertyDto { Id = saleProperty.Id, ListingType = PropertyListingType.Sale },
                new PropertyDto { Id = bothProperty.Id, ListingType = PropertyListingType.Both }
            };

            A.CallTo(() => _mapper.Map<ICollection<PropertyDto>>(A<ICollection<Property>>._))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetPropertiesForSaleAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
        }

        [Fact]
        public async Task SearchPropertiesAsync_WithLocationFilter_ShouldFilterByLocation()
        {
            // Arrange
            var property1 = await CreateTestPropertyDirectly(locationId: 1);
            var property2 = await CreateTestPropertyDirectly(locationId: 2);

            var searchCriteria = new PropertySearchDto { Location = "City 1" };

            A.CallTo(() => _mapper.Map<PropertyDto>(A<Property>.That.Matches(p => p.LocationId == 1)))
                .Returns(new PropertyDto { Id = property1.Id });

            // Act
            var result = await _service.SearchPropertiesAsync(searchCriteria);

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items.First().Id.ShouldBe(property1.Id);
        }

        // Helper methods
        private async Task SetupTestData()
        {
            // Create test category
            var category = new Category { Id = 1, Name = "Test Category" };
            await _dbContext.Categories.AddAsync(category);

            // Create test location
            var location = new Location { Id = 1, City = "Test City", Address = "Test Address" };
            await _dbContext.Locations.AddAsync(location);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<Property> CreateTestPropertyDirectly(
            string userId = "test-owner",
            PropertyListingType listingType = PropertyListingType.Sale,
            int locationId = 1,
            decimal price = 100000)
        {
            // 1. Create Category first
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == 1);
            if (category == null)
            {
                category = new Category { Id = 1, Name = "Test Category" };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
            }

            // 2. Create Location
            var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
            if (location == null)
            {
                location = new Location
                {
                    Id = locationId,
                    City = $"City {locationId}",
                    Address = $"Address {locationId}",
                    IsDeleted = false
                };
                await _dbContext.Locations.AddAsync(location);
                await _dbContext.SaveChangesAsync();
            }

            // 3. CREATE THE OWNER/USER - This was missing!
            var owner = await _dbContext.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
            if (owner == null)
            {
                owner = new ApplicationUser
                {
                    Id = userId,
                    UserName = userId,
                    Email = $"{userId}@test.com",
                    EmailConfirmed = true,
                    IsDeleted = false
                };
                await _dbContext.ApplicationUser.AddAsync(owner);
                await _dbContext.SaveChangesAsync();
            }

            // 4. Now create the Property with all valid foreign keys
            var property = new Property
            {
                Title = $"Test Property {Guid.NewGuid()}",
                Description = "Test Description",
                Price = price,
                Area = 100,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                OwnerId = userId,  // This now references a real user
                CategoryId = 1,    // This references a real category
                LocationId = locationId,  // This references a real location
                ListingType = listingType,
                PropertyType = PropertyType.TwoBedrooms
            };

            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();

            return property;
        }

        private PropertyDto CreateTestPropertyDto(Property property)
        {
            return new PropertyDto
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Area = property.Area,
                CreatedOn = property.CreatedOn,
                OwnerId = property.OwnerId,
                CategoryId = property.CategoryId,
                LocationId = property.LocationId,
                ListingType = property.ListingType
            };
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}