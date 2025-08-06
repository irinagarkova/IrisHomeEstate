using AutoMapper;
using FakeItEasy;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace HomeEstate.Services.Core.Tests.Services
{
    public class ApplicationUserServiceTests : IDisposable
    {
        private readonly HomeEstateDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationUserService _service;

        public ApplicationUserServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<HomeEstateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new HomeEstateDbContext(options);

            _dbContext.Properties.RemoveRange(_dbContext.Properties);
            _dbContext.Categories.RemoveRange(_dbContext.Categories);
            _dbContext.Locations.RemoveRange(_dbContext.Locations);
            _dbContext.PropertyImages.RemoveRange(_dbContext.PropertyImages);
            _dbContext.FavoriteProperties.RemoveRange(_dbContext.FavoriteProperties);
            _dbContext.SaveChanges();
            // Setup fake dependencies
            _mapper = A.Fake<IMapper>();
            _userManager = A.Fake<UserManager<ApplicationUser>>();
            _roleManager = A.Fake<RoleManager<IdentityRole>>();

            _service = new ApplicationUserService(_dbContext, _mapper, _userManager, _roleManager);
        }

        [Fact]
        public async Task GetApplicationUser_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new ApplicationUser
            {
                Id = "test-id",
                Email = email,
                UserName = email
            };

            await _dbContext.ApplicationUser.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetApplicationUser(email);

            // Assert
            result.ShouldNotBeNull();
            result.Email.ShouldBe(email);
            result.Id.ShouldBe("test-id");
        }

        [Fact]
        public async Task GetApplicationUser_WhenUserDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act & Assert
            await Should.ThrowAsync<Exception>(() => _service.GetApplicationUser(email));
        }

        [Fact]
        public async Task UpdateApplicationUser_WhenUserExists_ShouldUpdateSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            var existingUser = new ApplicationUser
            {
                Id = userId,
                Email = "old@example.com",
                UserName = "oldusername"
            };

            var userDto = new ApplicationUserDto
            {
                Id = userId,
                Email = "new@example.com",
                UserName = "newusername"
            };

            await _dbContext.ApplicationUser.AddAsync(existingUser);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.UpdateApplicationUser(userDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(userId);
        }
        [Fact]
        public async Task UpdateApplicationUser_WhenUserDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var userDto = new ApplicationUserDto
            {
                Id = "nonexistent-id",
                Email = "test@example.com"
            };

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(() => _service.UpdateApplicationUser(userDto));
            exception.Message.ShouldContain("not found");
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserExists_ShouldDeleteSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "test@example.com"
            };

            await _dbContext.ApplicationUser.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.DeleteUserAsync(userId);

            // Assert
            var deletedUser = await _dbContext.ApplicationUser.FindAsync(userId);
            deletedUser.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var userId = "nonexistent-id";

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(() => _service.DeleteUserAsync(userId));
            exception.Message.ShouldContain("not found");
        }

        //[Fact]
        //public async Task GetAllRolesAsync_ShouldReturnAllRoles()
        //{
        //    // Arrange
        //    var roles = new List<IdentityRole>
        //    {
        //        new IdentityRole { Name = "Admin" },
        //        new IdentityRole { Name = "User" }
        //    };

        //    A.CallTo(() => _roleManager.Roles).Returns(roles.AsQueryable());

        //    // Act
        //    var result = await _service.GetAllRolesAsync();

        //    // Assert
        //    result.ShouldNotBeNull();
        //    result.Count.ShouldBe(2);
        //    result.ShouldContain("Admin");
        //    result.ShouldContain("User");
        //}

        [Fact]
        public async Task GetUserRolesAsync_WhenUserExists_ShouldReturnUserRoles()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new ApplicationUser { Id = userId };
            var roles = new List<string> { "Admin", "User" };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(roles);

            // Act
            var result = await _service.GetUserRolesAsync(userId);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldContain("Admin");
            result.ShouldContain("User");
        }

        [Fact]
        public async Task GetUserRolesAsync_WhenUserDoesNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = "nonexistent-id";
            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns((ApplicationUser)null);

            // Act
            var result = await _service.GetUserRolesAsync(userId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task AddUserToRoleAsync_WhenUserExistsAndRoleExists_ShouldAddSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            var roleName = "Admin";
            var user = new ApplicationUser { Id = userId };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _roleManager.RoleExistsAsync(roleName)).Returns(true);
            A.CallTo(() => _userManager.IsInRoleAsync(user, roleName)).Returns(false);
            A.CallTo(() => _userManager.AddToRoleAsync(user, roleName)).Returns(IdentityResult.Success);

            // Act
            var result = await _service.AddUserToRoleAsync(userId, roleName);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            A.CallTo(() => _userManager.AddToRoleAsync(user, roleName)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddUserToRoleAsync_WhenUserDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var userId = "nonexistent-id";
            var roleName = "Admin";

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns((ApplicationUser)null);

            // Act
            var result = await _service.AddUserToRoleAsync(userId, roleName);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.Description == "User not found");
        }

        [Fact]
        public async Task GetTotalUsersCount_ShouldReturnCorrectCount()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", Email = "user1@test.com" },
                new ApplicationUser { Id = "2", Email = "user2@test.com" },
                new ApplicationUser { Id = "3", Email = "user3@test.com" }
            };

            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetTotalUsersCount();

            // Assert
            result.ShouldBe(3);
        }

        [Fact]
        public async Task GetRecentUsers_ShouldReturnCorrectNumberOfUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", Email = "user1@test.com" },
                new ApplicationUser { Id = "2", Email = "user2@test.com" },
                new ApplicationUser { Id = "3", Email = "user3@test.com" },
                new ApplicationUser { Id = "4", Email = "user4@test.com" }
            };

            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            var userDtos = users.TakeLast(2).Select(u => new ApplicationUserDto
            {
                Id = u.Id,
                Email = u.Email
            }).ToList();

            A.CallTo(() => _mapper.Map<List<ApplicationUserDto>>(A<List<ApplicationUser>>._))
                .Returns(userDtos);

            // Act
            var result = await _service.GetRecentUsers(2);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            A.CallTo(() => _mapper.Map<List<ApplicationUserDto>>(A<List<ApplicationUser>>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IsUserInRoleAsync_WhenUserInRole_ShouldReturnTrue()
        {
            // Arrange
            var userId = "test-user-id";
            var roleName = "Admin";
            var user = new ApplicationUser { Id = userId };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.IsInRoleAsync(user, roleName)).Returns(true);

            // Act
            var result = await _service.IsUserInRoleAsync(userId, roleName);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task IsUserInRoleAsync_WhenUserNotInRole_ShouldReturnFalse()
        {
            // Arrange
            var userId = "test-user-id";
            var roleName = "Admin";
            var user = new ApplicationUser { Id = userId };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.IsInRoleAsync(user, roleName)).Returns(false);

            // Act
            var result = await _service.IsUserInRoleAsync(userId, roleName);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task IsUserInRoleAsync_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var userId = "nonexistent-id";
            var roleName = "Admin";

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns((ApplicationUser)null);

            // Act
            var result = await _service.IsUserInRoleAsync(userId, roleName);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task RemoveUserFromRoleAsync_WhenNotLastAdmin_ShouldRemoveSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            var roleName = "User"; // Not admin role
            var user = new ApplicationUser { Id = userId };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.RemoveFromRoleAsync(user, roleName)).Returns(IdentityResult.Success);

            // Act
            var result = await _service.RemoveUserFromRoleAsync(userId, roleName);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            A.CallTo(() => _userManager.RemoveFromRoleAsync(user, roleName)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RemoveUserFromRoleAsync_WhenLastAdmin_ShouldReturnFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var roleName = "Admin";
            var user = new ApplicationUser { Id = userId };
            var adminUsers = new List<ApplicationUser> { user }; // Only one admin

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.GetUsersInRoleAsync("Admin")).Returns(adminUsers);

            // Act
            var result = await _service.RemoveUserFromRoleAsync(userId, roleName);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.Description.Contains("Cannot remove the last admin"));
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}