using AutoMapper;
using FakeItEasy;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Services;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace HomeEstate.Services.Core.Tests.Services
{
    public class CategoryServiceTests : IDisposable
    {
        private readonly HomeEstateDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly CategoryService _service;

        public CategoryServiceTests()
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
            // Setup fake mapper
            _mapper = A.Fake<IMapper>();

            _service = new CategoryService(_dbContext, _mapper);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_WhenCategoriesExist_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Apartment" },
                new Category { Id = 2, Name = "House" },
                new Category { Id = 3, Name = "Office" }
            };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            // Setup mapper to return DTOs for each category
            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Id == 1)))
                .Returns(categoryDtos[0]);
            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Id == 2)))
                .Returns(categoryDtos[1]);
            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Id == 3)))
                .Returns(categoryDtos[2]);

            // Act
            var result = await _service.GetAllCategoriesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result.ShouldContain(c => c.Name == "Apartment");
            result.ShouldContain(c => c.Name == "House");
            result.ShouldContain(c => c.Name == "Office");

            // Verify mapper was called for each category
            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>._)).MustHaveHappened(3, Times.Exactly);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_WhenNoCategoriesExist_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _service.GetAllCategoriesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnCategoriesOrderedById()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 3, Name = "Office" },
                new Category { Id = 1, Name = "Apartment" },
                new Category { Id = 2, Name = "House" }
            };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Apartment" },
                new CategoryDto { Id = 2, Name = "House" },
                new CategoryDto { Id = 3, Name = "Office" }
            };

            // Setup mapper
            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Id == 1)))
                .Returns(categoryDtos[0]);
            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Id == 2)))
                .Returns(categoryDtos[1]);
            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Id == 3)))
                .Returns(categoryDtos[2]);

            // Act
            var result = await _service.GetAllCategoriesAsync();

            // Assert
            result.ShouldNotBeNull();
            var resultList = result.ToList();
            resultList[0].Id.ShouldBe(1);
            resultList[1].Id.ShouldBe(2);
            resultList[2].Id.ShouldBe(3);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WhenCategoryExists_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Apartment" };
            var categoryDto = new CategoryDto { Id = 1, Name = "Apartment" };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Id == 1)))
                .Returns(categoryDto);

            // Act
            var result = await _service.GetCategoryByIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Apartment");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = 999;

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(() => _service.GetCategoryByIdAsync(nonExistentId));
            exception.Message.ShouldContain("Category");
            exception.Message.ShouldContain(nonExistentId.ToString());
        }

        [Fact]
        public async Task GetCategoryByNameAsync_WhenCategoryExists_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Apartment" };
            var categoryDto = new CategoryDto { Id = 1, Name = "Apartment" };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Name == "Apartment")))
                .Returns(categoryDto);

            // Act
            var result = await _service.GetCategoryByNameAsync("Apartment");

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Apartment");
        }

        [Fact]
        public async Task GetCategoryByNameAsync_WhenCategoryExistsWithDifferentCase_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Apartment" };
            var categoryDto = new CategoryDto { Id = 1, Name = "Apartment" };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            A.CallTo(() => _mapper.Map<CategoryDto>(A<Category>.That.Matches(c => c.Name == "Apartment")))
                .Returns(categoryDto);

            // Act
            var result = await _service.GetCategoryByNameAsync("APARTMENT"); // Different case

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Apartment");
        }

        [Fact]
        public async Task GetCategoryByNameAsync_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentName = "NonExistentCategory";

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(() => _service.GetCategoryByNameAsync(nonExistentName));
            exception.Message.ShouldContain("Category");
            exception.Message.ShouldContain(nonExistentName);
        }

        [Fact]
        public async Task GetCategoryByNameAsync_WithEmptyName_ShouldThrowNotFoundException()
        {
            // Act & Assert
            var exception = await Should.ThrowAsync<ArgumentException>(() => _service.GetCategoryByNameAsync(""));
        }

        [Fact]
        public async Task GetCategoryByNameAsync_WithNullName_ShouldThrowNotFoundException()
        {
            // Act & Assert
            var exception = await Should.ThrowAsync<ArgumentException>(() => _service.GetCategoryByNameAsync(null));
        }

        [Fact]
        public async Task GetCategoryByNameAsync_WithWhitespaceOnly_ShouldThrowNotFoundException()
        {
            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(() => _service.GetCategoryByNameAsync("   "));
            exception.Message.ShouldContain("Category");
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}