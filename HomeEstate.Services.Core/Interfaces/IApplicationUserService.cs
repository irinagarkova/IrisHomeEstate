using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using Microsoft.AspNetCore.Identity;

namespace HomeEstate.Services.Core.Interfaces
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser> GetApplicationUser(string email); 
        Task<ApplicationUser> UpdateApplicationUser (ApplicationUserDto applicationUser);
        Task<int> GetTotalUsersCount();
        Task<List<ApplicationUser>> GetRecentUsers(int count);
        Task<PaginatedDto<ApplicationUserDto>> GetAllUsersAsync(int page, int pageSize);
        Task DeleteUserAsync(string id);
        Task<PaginatedDto<ApplicationUserWithRoleDto>> GetAllUsersWithRolesAsync(int page, int pageSize);
        Task<List<string>> GetAllRolesAsync();
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
        Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<IdentityResult> UpdateUserRolesAsync(string userId, List<string> newRoles);
        Task<ApplicationUserWithRoleDto> GetUserWithRolesAsync(string userId);
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
        Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName);
    }
}
