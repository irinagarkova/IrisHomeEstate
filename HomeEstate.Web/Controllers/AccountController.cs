using AutoMapper;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HomeEstate.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IApplicationUserService accountService;
        private readonly IPropertyService propertyService;
        private readonly IFavoritePropertyService favoritePropertyService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IApplicationUserService applicationUserService,
            IPropertyService propertyService,
            IFavoritePropertyService favoritePropertyService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            this.accountService = applicationUserService;
            this.propertyService = propertyService;
            this.favoritePropertyService = favoritePropertyService;
            this.mapper = mapper;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await accountService.GetApplicationUser(User.Identity.Name);
                if (user == null)
                {
                    logger.LogWarning("User not found: {UserName}", User.Identity.Name);
                    return NotFound("User not found");
                }

                var dto = mapper.Map<ApplicationUserDto>(user);
                var userViewModel = mapper.Map<ApplicationUserViewModel>(dto);
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading user profile for {UserName}", User.Identity.Name);
                TempData["Error"] = "Unable to load your profile.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Edit()
        {
            try
            {
                var user = await accountService.GetApplicationUser(User.Identity.Name);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                var dto = mapper.Map<ApplicationUserDto>(user);
                var userViewModel = mapper.Map<ApplicationUserViewModel>(dto);
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading profile for editing for {UserName}", User.Identity.Name);
                TempData["Error"] = "Unable to load profile for editing.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> GetUserStatistics()
        {
            var user = await accountService.GetApplicationUser(User.Identity.Name);
            return Json(new { totalProperties = user.Properties.Count, favoritteProperties = user.FavoriteProperties.Count });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var currentUser = await accountService.GetApplicationUser(User.Identity.Name);
                if (currentUser == null || currentUser.Id != model.Id)
                {
                    logger.LogWarning("Unauthorized profile edit attempt by {UserName}", User.Identity.Name);
                    return Forbid();
                }

                var userDto = mapper.Map<ApplicationUserDto>(model);
                await accountService.UpdateApplicationUser(userDto);

                logger.LogInformation("Profile updated successfully for user {UserId}", model.Id);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating profile for user {UserId}", model.Id);
                ModelState.AddModelError("", "An error occurred while updating your profile.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserActivity()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var activities = new[]
                {
                    new {
                        date = DateTime.Now.AddHours(-2),
                        type = "property_added",
                        description = "Added new property: 'Luxury Apartment in Sofia Center'",
                        icon = "fas fa-plus",
                        color = "#4e73df"
                    },
                    new {
                        date = DateTime.Now.AddDays(-1),
                        type = "favorites_added",
                        description = "Added 3 properties to favorites",
                        icon = "fas fa-heart",
                        color = "#1cc88a"
                    },
                    new {
                        date = DateTime.Now.AddDays(-3),
                        type = "profile_updated",
                        description = "Updated profile information",
                        icon = "fas fa-edit",
                        color = "#36b9cc"
                    },
                    new {
                        date = DateTime.Now.AddDays(-7),
                        type = "login",
                        description = "Logged in from new device",
                        icon = "fas fa-sign-in-alt",
                        color = "#f6c23e"
                    }
                };

                return Json(new { success = true, data = activities });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user activity for {UserName}", User.Identity.Name);
                return Json(new { success = false, message = "Error loading activity" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var user = await accountService.GetApplicationUser(User.Identity.Name);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                await accountService.DeleteUserAsync(user.Id);

                logger.LogInformation("User account deleted: {UserId}", user.Id);

                await HttpContext.SignOutAsync();

                TempData["Success"] = "Your account has been deleted successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting account for {UserName}", User.Identity.Name);
                TempData["Error"] = "An error occurred while deleting your account.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var user = await accountService.GetApplicationUser(User.Identity.Name);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var userProperties = await propertyService.GetPropertiesByUserIdAsync(userId);
                var recentProperties = userProperties
                    .OrderByDescending(p => p.CreatedOn)
                    .Take(5)
                    .Select(p => mapper.Map<PropertyViewModel>(p))
                    .ToList();

                var userFavorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(User.Identity.Name);
                var recentFavorites = userFavorites
                    .Take(5)
                    .Select(f => mapper.Map<PropertyViewModel>(f.Property))
                    .ToList();

                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);

                var dashboardData = new
                {
                    User = mapper.Map<ApplicationUserViewModel>(user),
                    RecentProperties = recentProperties,
                    RecentFavorites = recentFavorites,
                    Statistics = statistics
                };

                ViewBag.DashboardData = dashboardData;
                return View(mapper.Map<ApplicationUserViewModel>(user));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading dashboard for {UserName}", User.Identity.Name);
                TempData["Error"] = "Unable to load dashboard.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    logger.LogInformation("Password changed successfully for user {UserId}", user.Id);
                    return Json(new { success = true, message = "Password changed successfully" });
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return Json(new { success = false, errors = errors });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error changing password for {UserName}", User.Identity.Name);
                return Json(new { success = false, message = "An error occurred while changing password" });
            }
        }
    }
}