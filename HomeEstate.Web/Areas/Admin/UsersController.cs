using AutoMapper;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Areas.Models;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IApplicationUserService applicationUserService;
        private readonly IMapper mapper;
        private readonly ILogger<UsersController> logger;

        public UsersController(
            IApplicationUserService applicationUserService,
            IMapper mapper,
            ILogger<UsersController> logger)
        {
            this.applicationUserService = applicationUserService;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = "", string roleFilter = "")
        {
            try
            {
                // Validate pagination parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(Math.Max(1, pageSize), 50);

                var users = await applicationUserService.GetAllUsersWithRolesAsync(page, pageSize);

                // Apply filters if needed (you might want to add these methods to the service)
                // For now, we'll work with what we have

                var viewModel = new AdminUsersViewModel
                {
                    Users = users,
                    SearchTerm = searchTerm,
                    RoleFilter = roleFilter,
                    PageSizes = new[] { 5, 10, 25, 50 },
                    AvailableRoles = await applicationUserService.GetAllRolesAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading users for admin");
                TempData["Error"] = "Unable to load users.";
                return View(new AdminUsersViewModel());
            }
        }

        // AJAX endpoint for loading users
        [HttpGet]
        public async Task<IActionResult> LoadUsers(int page = 1, int pageSize = 10, string searchTerm = "", string roleFilter = "")
        {
            try
            {
                page = Math.Max(1, page);
                pageSize = Math.Min(Math.Max(1, pageSize), 50);

                var users = await applicationUserService.GetAllUsersWithRolesAsync(page, pageSize);

                return Json(new
                {
                    success = true,
                    data = users.Items,
                    pagination = new
                    {
                        currentPage = users.CurrentPage,
                        totalPages = users.TotalPages,
                        totalItems = users.TotalItems,
                        pageSize = users.PageSize,
                        hasPreviousPage = users.HasPreviousPage,
                        hasNextPage = users.HasNextPage
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading users via AJAX");
                return Json(new { success = false, message = "Error loading users" });
            }
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction("Index");
            }

            try
            {
                var user = await applicationUserService.GetUserWithRolesAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading user details. ID: {UserId}", id);
                TempData["Error"] = "Unable to load user details.";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction("Index");
            }

            try
            {
                var user = await applicationUserService.GetUserWithRolesAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                var viewModel = new EditUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    SelectedRoles = user.Roles,
                    AvailableRoles = await applicationUserService.GetAllRolesAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading user for edit. ID: {UserId}", id);
                TempData["Error"] = "Unable to load user for editing.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await applicationUserService.GetAllRolesAsync();
                return View(model);
            }

            try
            {
                // Update user roles
                var result = await applicationUserService.UpdateUserRolesAsync(model.Id, model.SelectedRoles);

                if (result.Succeeded)
                {
                    logger.LogInformation("User roles updated by admin. User ID: {UserId}", model.Id);
                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating user. ID: {UserId}", model.Id);
                ModelState.AddModelError("", "An error occurred while updating the user.");
            }

            model.AvailableRoles = await applicationUserService.GetAllRolesAsync();
            return View(model);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction("Index");
            }

            try
            {
                var user = await applicationUserService.GetUserWithRolesAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading user for deletion. ID: {UserId}", id);
                TempData["Error"] = "Unable to find user.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                // Prevent admin from deleting themselves
                var currentUser = User.Identity?.Name;
                var userToDelete = await applicationUserService.GetApplicationUser(id);

                if (userToDelete?.Email == currentUser)
                {
                    TempData["Error"] = "You cannot delete your own account.";
                    return RedirectToAction("Index");
                }

                // Check if this is the last admin
                var userRoles = await applicationUserService.GetUserRolesAsync(id);
                if (userRoles.Contains("Admin"))
                {
                    var admins = await applicationUserService.GetUsersInRoleAsync("Admin");
                    if (admins.Count <= 1)
                    {
                        TempData["Error"] = "Cannot delete the last admin user.";
                        return RedirectToAction("Index");
                    }
                }

                await applicationUserService.DeleteUserAsync(id);
                logger.LogInformation("User deleted by admin. ID: {UserId}", id);
                TempData["Success"] = "User deleted successfully.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting user. ID: {UserId}", id);
                TempData["Error"] = "An error occurred while deleting the user.";
            }

            return RedirectToAction("Index");
        }

        // AJAX DELETE endpoint
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(string id)
        {
            try
            {
                // Prevent admin from deleting themselves
                var currentUser = User.Identity?.Name;
                var userToDelete = await applicationUserService.GetApplicationUser(id);

                if (userToDelete?.Email == currentUser)
                {
                    return Json(new
                    {
                        success = false,
                        message = "You cannot delete your own account"
                    });
                }

                // Check if this is the last admin
                var userRoles = await applicationUserService.GetUserRolesAsync(id);
                if (userRoles.Contains("Admin"))
                {
                    var admins = await applicationUserService.GetUsersInRoleAsync("Admin");
                    if (admins.Count <= 1)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Cannot delete the last admin user"
                        });
                    }
                }

                await applicationUserService.DeleteUserAsync(id);
                logger.LogInformation("User deleted via AJAX by admin. ID: {UserId}", id);

                return Json(new
                {
                    success = true,
                    message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting user via AJAX. ID: {UserId}", id);
                return Json(new
                {
                    success = false,
                    message = "Error deleting user"
                });
            }
        }

        // Role management endpoints
        [HttpPost]
        public async Task<IActionResult> AddRole(string userId, string roleName)
        {
            try
            {
                var result = await applicationUserService.AddUserToRoleAsync(userId, roleName);

                if (result.Succeeded)
                {
                    logger.LogInformation("Role {RoleName} added to user {UserId} by admin", roleName, userId);
                    return Json(new { success = true, message = $"Role '{roleName}' added successfully" });
                }

                return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "Error adding role" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding role {RoleName} to user {UserId}", roleName, userId);
                return Json(new { success = false, message = "Error adding role" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            try
            {
                var result = await applicationUserService.RemoveUserFromRoleAsync(userId, roleName);

                if (result.Succeeded)
                {
                    logger.LogInformation("Role {RoleName} removed from user {UserId} by admin", roleName, userId);
                    return Json(new { success = true, message = $"Role '{roleName}' removed successfully" });
                }

                return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "Error removing role" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", roleName, userId);
                return Json(new { success = false, message = "Error removing role" });
            }
        }

        // Bulk operations
        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromBody] string[] userIds)
        {
            try
            {
                if (userIds == null || userIds.Length == 0)
                {
                    return Json(new { success = false, message = "No users selected" });
                }

                var currentUser = User.Identity?.Name;
                var deletedCount = 0;
                var errors = new List<string>();

                foreach (var id in userIds)
                {
                    try
                    {
                        var userToDelete = await applicationUserService.GetApplicationUser(id);

                        // Skip current user
                        if (userToDelete?.Email == currentUser)
                        {
                            errors.Add($"Skipped {userToDelete.Email} (cannot delete own account)");
                            continue;
                        }

                        // Check if this is an admin
                        var userRoles = await applicationUserService.GetUserRolesAsync(id);
                        if (userRoles.Contains("Admin"))
                        {
                            var admins = await applicationUserService.GetUsersInRoleAsync("Admin");
                            if (admins.Count <= 1)
                            {
                                errors.Add($"Skipped {userToDelete?.Email} (last admin)");
                                continue;
                            }
                        }

                        await applicationUserService.DeleteUserAsync(id);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error deleting user in bulk operation. ID: {UserId}", id);
                        errors.Add($"Error deleting user {id}");
                    }
                }

                logger.LogInformation("Bulk delete operation completed. Deleted {Count} users", deletedCount);

                var message = $"Successfully deleted {deletedCount} users";
                if (errors.Any())
                {
                    message += $". {errors.Count} errors occurred.";
                }

                return Json(new
                {
                    success = true,
                    message = message,
                    deletedCount = deletedCount,
                    errors = errors
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in bulk delete operation");
                return Json(new
                {
                    success = false,
                    message = "Error performing bulk delete"
                });
            }
        }
    }
}