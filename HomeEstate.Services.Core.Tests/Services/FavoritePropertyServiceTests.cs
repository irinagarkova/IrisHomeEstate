using AutoMapper;
using FakeItEasy;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Services;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace HomeEstate.Services.Core.Tests.Services
{
    public class FavoritePropertyServiceTests : IDisposable
    {
        private readonly HomeEstateDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IPropertyService _propertyService;
        private readonly FavoritePropertyService _service;

        public FavoritePropertyServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<HomeEstateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _dbContext = new HomeEstateDbContext(options);

            // Ensure database is created
            _dbContext.Database.EnsureCreated();
            _dbContext.Properties.RemoveRange(_dbContext.Properties);
            _dbContext.Categories.RemoveRange(_dbContext.Categories);
            _dbContext.Locations.RemoveRange(_dbContext.Locations);
            _dbContext.PropertyImages.RemoveRange(_dbContext.PropertyImages);
            _dbContext.FavoriteProperties.RemoveRange(_dbContext.FavoriteProperties);
            _dbContext.SaveChanges();
            // Setup fake dependencies
            _mapper = A.Fake<IMapper>();
            _applicationUserService = A.Fake<IApplicationUserService>();
            _propertyService = A.Fake<IPropertyService>();

            _service = new FavoritePropertyService(_dbContext, _mapper, _applicationUserService, _propertyService);
        }

        [Fact]
        public async Task AddPropertyToFavoriteAsync_WhenPropertyAndUserExist_ShouldAddToFavorites()
        {
            // Arrange
            var propertyId = 1;
            var userEmail = "test@example.com";
            var userId = "user-id";

            var user = new ApplicationUser { Id = userId, Email = userEmail };
            var propertyDto = new PropertyDto { Id = propertyId, Title = "Test Property" };

            A.CallTo(() => _propertyService.GetPropertyAsync(propertyId)).Returns(propertyDto);
            A.CallTo(() => _applicationUserService.GetApplicationUser(userEmail)).Returns(user);

            // Act
            await _service.AddPropertyToFavoriteAsync(propertyId, userEmail);

            // Assert
            var favoriteProperty = await _dbContext.FavoriteProperties
                .FirstOrDefaultAsync(fp => fp.PropertyId == propertyId && fp.UserId == userId);

            favoriteProperty.ShouldNotBeNull();
            favoriteProperty.PropertyId.ShouldBe(propertyId);
            favoriteProperty.UserId.ShouldBe(userId);
        }

        [Fact]
        public async Task AddPropertyToFavoriteAsync_WhenAlreadyInFavorites_ShouldThrowException()
        {
            // Arrange
            var propertyId = 1;
            var userEmail = "test@example.com";
            var userId = "user-id";

            var user = new ApplicationUser { Id = userId, Email = userEmail };
            var propertyDto = new PropertyDto { Id = propertyId, Title = "Test Property" };

            // Add existing favorite
            var existingFavorite = new FavoriteProperty
            {
                PropertyId = propertyId,
                UserId = userId
            };
            await _dbContext.FavoriteProperties.AddAsync(existingFavorite);
            await _dbContext.SaveChangesAsync();

            A.CallTo(() => _propertyService.GetPropertyAsync(propertyId)).Returns(propertyDto);
            A.CallTo(() => _applicationUserService.GetApplicationUser(userEmail)).Returns(user);

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(() => _service.AddPropertyToFavoriteAsync(propertyId, userEmail));
            exception.Message.ShouldContain("already have this property in favorite");
        }

        [Fact]
        public async Task RemovePropertyFromFavoriteAsync_WhenFavoriteExists_ShouldRemoveFromFavorites()
        {
            // Arrange
            var propertyId = 1;
            var userEmail = "test@example.com";
            var userId = "user-id";

            var user = new ApplicationUser { Id = userId, Email = userEmail };

            // Add favorite to remove
            var favoriteProperty = new FavoriteProperty
            {
                PropertyId = propertyId,
                UserId = userId
            };
            await _dbContext.FavoriteProperties.AddAsync(favoriteProperty);
            await _dbContext.SaveChangesAsync();

            A.CallTo(() => _applicationUserService.GetApplicationUser(userEmail)).Returns(user);

            // Act
            await _service.RemovePropertyFromFavoriteAsync(propertyId, userEmail);

            // Assert
            var removedFavorite = await _dbContext.FavoriteProperties
                .FirstOrDefaultAsync(fp => fp.PropertyId == propertyId && fp.UserId == userId);

            removedFavorite.ShouldBeNull();
        }

        [Fact]
        public async Task RemovePropertyFromFavoriteAsync_WhenFavoriteDoesNotExist_ShouldNotThrowException()
        {
            // Arrange
            var propertyId = 1;
            var userEmail = "test@example.com";
            var userId = "user-id";

            var user = new ApplicationUser { Id = userId, Email = userEmail };

            A.CallTo(() => _applicationUserService.GetApplicationUser(userEmail)).Returns(user);

            // Act & Assert
            await Should.NotThrowAsync(() => _service.RemovePropertyFromFavoriteAsync(propertyId, userEmail));
        }

        [Fact]
        public async Task GetFavoriteCountForPropertyAsync_WhenFavoritesExist_ShouldReturnCorrectCount()
        {
            // Arrange
            var propertyId = 1;
            var favorites = new List<FavoriteProperty>
            {
                new FavoriteProperty { PropertyId = propertyId, UserId = "user1" },
                new FavoriteProperty { PropertyId = propertyId, UserId = "user2" },
                new FavoriteProperty { PropertyId = propertyId, UserId = "user3" }
            };

            await _dbContext.FavoriteProperties.AddRangeAsync(favorites);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetFavoriteCountForPropertyAsync(propertyId);

            // Assert
            result.ShouldBe(3);
        }

        [Fact]
        public async Task GetFavoriteCountForPropertyAsync_WhenNoFavoritesExist_ShouldReturnZero()
        {
            // Arrange
            var propertyId = 999; // Non-existent property

            // Act
            var result = await _service.GetFavoriteCountForPropertyAsync(propertyId);

            // Assert
            result.ShouldBe(0);
        }

        [Fact]
        public async Task GetAllFavoritePropertiesAsync_WhenFavoritesExist_ShouldReturnMappedFavorites()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userId = "user-id";

            var user = new ApplicationUser { Id = userId, Email = userEmail };
            var category = new Category { Id = 1, Name = "Apartment" };
            var location = new Location { Id = 1, City = "Sofia", Address = "Test Address" };
            var property = new Property
            {
                Id = 1,
                Title = "Test Property",
                CategoryId = 1,
                Category = category,
                LocationId = 1,
                Location = location,
                OwnerId = "owner-id",
                Description = "description"
            };

            var favoriteProperty = new FavoriteProperty
            {
                PropertyId = 1,
                UserId = userId,
                Property = property,
                User = user
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.Properties.AddAsync(property);
            await _dbContext.FavoriteProperties.AddAsync(favoriteProperty);
            await _dbContext.SaveChangesAsync();

            var expectedDto = new FavoritePropertyDto
            {
                PropertyId = 1,
                UserId = userId,
                Property = new PropertyDto { Id = 1, Title = "Test Property" },
                User = new ApplicationUserDto { Id = userId, Email = userEmail }
            };

            A.CallTo(() => _mapper.Map<FavoritePropertyDto>(A<FavoriteProperty>._)).Returns(expectedDto);

            // Act
            var result = await _service.GetAllFavoritePropertiesAsync(userEmail);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result.First().PropertyId.ShouldBe(1);
            result.First().UserId.ShouldBe(userId);

            A.CallTo(() => _mapper.Map<FavoritePropertyDto>(A<FavoriteProperty>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetAllFavoritePropertiesAsync_WhenNoFavoritesExist_ShouldReturnEmptyCollection()
        {
            // Arrange
            var userEmail = "test@example.com";

            // Act
            var result = await _service.GetAllFavoritePropertiesAsync(userEmail);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAllFavoritePropertiesAsync_ShouldIncludePropertyDetails()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userId = "user-id";

            var user = new ApplicationUser { Id = userId, Email = userEmail };
            var category = new Category { Id = 1, Name = "House" };
            var location = new Location { Id = 1, City = "Plovdiv", Address = "Test Address" };

            var property = new Property
            {
                Id = 1,
                Title = "Beautiful House",
                CategoryId = 1,
                Category = category,
                LocationId = 1,
                Location = location,
                OwnerId = "owner-id",
                Description = "description",
                Images = new List<PropertyImage>
                {
                    new PropertyImage { Id = 1, PropertyId = 1, ImageUrl = "test.jpg" }
                }
            };

            var favoriteProperty = new FavoriteProperty
            {
                PropertyId = 1,
                UserId = userId,
                Property = property,
                User = user
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.Properties.AddAsync(property);
            await _dbContext.FavoriteProperties.AddAsync(favoriteProperty);
            await _dbContext.SaveChangesAsync();

            var expectedDto = new FavoritePropertyDto
            {
                PropertyId = 1,
                UserId = userId
            };

            A.CallTo(() => _mapper.Map<FavoritePropertyDto>(A<FavoriteProperty>._)).Returns(expectedDto);

            // Act
            var result = await _service.GetAllFavoritePropertiesAsync(userEmail);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);

            // Verify that the query included the necessary relationships
            A.CallTo(() => _mapper.Map<FavoritePropertyDto>(
                A<FavoriteProperty>.That.Matches(fp =>
                    fp.Property != null &&
                    fp.Property.Images != null &&
                    fp.Property.Location != null &&
                    fp.Property.Category != null)))
                .MustHaveHappenedOnceExactly();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}