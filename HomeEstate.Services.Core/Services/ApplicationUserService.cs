using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace HomeEstate.Services.Core.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly HomeEstateDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ApplicationUserService(HomeEstateDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<ApplicationUser> GetApplicationUser(string email)
        {
            var user = await dbContext.ApplicationUser
                .Include(p=>p.FavoriteProperties)
                .ThenInclude(x=>x.Property)
                .ThenInclude(x=>x.Images)
                .FirstOrDefaultAsync(x=> x.Email == email);
            if(user != null)
            {
                return user; 
            }
            throw new Exception();
        }
        public async Task<ApplicationUser> UpdateApplicationUser(ApplicationUserDto applicationUser)
        {
            var userFromDb = await dbContext.ApplicationUser.FindAsync(applicationUser.Id);
            if (userFromDb == null)
            {
                throw new Exception($"User with ID {applicationUser.Id} not found.");
            }

            mapper.Map(applicationUser, userFromDb);

            await dbContext.SaveChangesAsync();

            return userFromDb;
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await dbContext.ApplicationUser.FirstOrDefaultAsync(x=> x.Id == id);
            if (user == null)
            {
                throw new Exception($"User with ID {id} not found.");
            }

            dbContext.ApplicationUser.Remove(user);
            await dbContext.SaveChangesAsync();
        }

            public async Task<List<string>> GetAllRolesAsync()
            {
                return await roleManager.Roles
                    .Select(r => r.Name)
                    .ToListAsync();
            }

            public async Task<List<string>> GetUserRolesAsync(string userId)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null) return new List<string>();

                var roles = await userManager.GetRolesAsync(user);
                return roles.ToList();
            }

            public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                // Check if role exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Role does not exist" });
                }

                // Check if user is already in role
                if (await userManager.IsInRoleAsync(user, roleName))
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User is already in this role" });
                }

                return await userManager.AddToRoleAsync(user, roleName);
            }

            public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                // Prevent removing the last admin
                if (roleName == "Admin")
                {
                    var adminsCount = (await GetUsersInRoleAsync("Admin")).Count;
                    if (adminsCount <= 1)
                    {
                        return IdentityResult.Failed(new IdentityError
                        {
                            Description = "Cannot remove the last admin user"
                        });
                    }
                }

                return await userManager.RemoveFromRoleAsync(user, roleName);
            }

            public async Task<IdentityResult> UpdateUserRolesAsync(string userId, List<string> newRoles)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                // Get current roles
                var currentRoles = await userManager.GetRolesAsync(user);

                // Prevent removing admin role if this is the last admin
                if (currentRoles.Contains("Admin") && !newRoles.Contains("Admin"))
                {
                    var adminsCount = (await GetUsersInRoleAsync("Admin")).Count;
                    if (adminsCount <= 1)
                    {
                        return IdentityResult.Failed(new IdentityError
                        {
                            Description = "Cannot remove admin role from the last admin user"
                        });
                    }
                }

                // Remove current roles
                var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return removeResult;
                }

                // Add new roles
                if (newRoles.Any())
                {
                    var addResult = await userManager.AddToRolesAsync(user, newRoles);
                    return addResult;
                }

                return IdentityResult.Success;
            }

            public async Task<ApplicationUserWithRoleDto> GetUserWithRolesAsync(string userId)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null) return null;

                var roles = await userManager.GetRolesAsync(user);

                var userViewModel = mapper.Map<ApplicationUserWithRoleDto>(user);
                userViewModel.Roles = roles.ToList();
                userViewModel.LockoutEnd = user.LockoutEnd;

                return userViewModel;
            }

            public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null) return false;

                return await userManager.IsInRoleAsync(user, roleName);
            }

            public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName)
            {
                var users = await userManager.GetUsersInRoleAsync(roleName);
                return users.ToList();
            }

            // Enhanced existing method
            

        public async Task<int> GetTotalUsersCount()
        {
            var total = await dbContext.Users.CountAsync();
            return total;
        }

        public async Task<List<ApplicationUserDto>> GetRecentUsers(int count)
        {
            var recentUsers = await dbContext.Users.ToListAsync();
            var users = new List<ApplicationUser>();
            for (int i = recentUsers.Count - 1; i > recentUsers.Count - count; i--)
            {
                users.Add(recentUsers[i]);
            }
            var mapped = mapper.Map<List<ApplicationUserDto>>(users);
            return mapped;
        }
        public async Task<Pagination<ApplicationUserWithRoleDto>> GetAllUsersWithRolesAsync(int page, int pageSize)
        {
            // Validate pagination parameters
            page = Math.Max(1, page); // Ensure page is at least 1
            pageSize = Math.Min(Math.Max(1, pageSize), 100); // Ensure pageSize is between 1 and 100

            // Get total count for pagination info
            var totalUsers = await userManager.Users.CountAsync();

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            // Get paginated users from database
            var users = await userManager.Users
                .OrderBy(u => u.UserName) // Important: Always order for consistent paging
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Convert to DTOs and populate roles
            var userDtos = new List<ApplicationUserWithRoleDto>();

            foreach (var user in users)
            {
                var userDto = mapper.Map<ApplicationUserWithRoleDto>(user);

                // Populate roles for each user
                var roles = await userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();

                // Set lockout status

                userDtos.Add(userDto);
            }

            // Return paginated result
            return new Pagination<ApplicationUserWithRoleDto>
            {
                Items = userDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalUsers,
                TotalPages = totalPages
            };
        }

        public async Task<Pagination<ApplicationUserDto>> GetAllUsersAsync(int page, int pageSize)
        {
            page = Math.Max(1, page); // Ensure page is at least 1
            pageSize = Math.Min(Math.Max(1, pageSize), 100); // Ensure pageSize is between 1 and 100

            // Get total count for pagination info
            var totalUsers = await GetTotalUsersCount();

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            var users = await userManager.Users
               .OrderBy(u => u.UserName) // Important: Always order for consistent paging
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new Pagination<ApplicationUserDto>
            {
                Items = mapper.Map<List<ApplicationUserDto>>(users),
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
    }
}
