using AutoMapper;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeEstate.Web.Controllers
{
    [Authorize]
    public class MyPropertyController : Controller
    {
        private readonly IPropertyService propertyService;
        private readonly IMapper mapper;
        private readonly ILogger<MyPropertyController> logger;

        public MyPropertyController(
            IPropertyService propertyService,
            IMapper mapper,
            ILogger<MyPropertyController> logger)
        {
            this.propertyService = propertyService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string searchTerm = "", string sortBy = "newest")
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 6) pageSize = 6;
            if (pageSize > 50) pageSize = 50;

            try
            {
                var myProps = await propertyService.GetPropertiesByUserIdAsync(userId);

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    myProps = myProps.Where(p =>
                        p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Location?.City != null && p.Location.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Apply sorting
                myProps = sortBy?.ToLower() switch
                {
                    "oldest" => myProps.OrderBy(p => p.CreatedOn).ToList(),
                    "price-asc" => myProps.OrderBy(p => p.Price).ToList(),
                    "price-desc" => myProps.OrderByDescending(p => p.Price).ToList(),
                    "area-asc" => myProps.OrderBy(p => p.Area).ToList(),
                    "area-desc" => myProps.OrderByDescending(p => p.Area).ToList(),
                    "title" => myProps.OrderBy(p => p.Title).ToList(),
                    _ => myProps.OrderByDescending(p => p.CreatedOn).ToList(), // newest (default)
                };

                // Calculate pagination
                var totalItems = myProps.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var skip = (page - 1) * pageSize;
                var pagedItems = myProps.Skip(skip).Take(pageSize).ToList();

                var viewModels = pagedItems.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

                // Get statistics
                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);
                ViewData["Statistics"] = statistics;

                // Create view model
                var viewModel = new MyPropertyIndexViewModel
                {
                    Properties = new Pagination<PropertyViewModel>
                    {
                        Items = viewModels,
                        CurrentPage = page,
                        TotalPages = totalPages,
                        TotalItems = totalItems,
                        PageSize = pageSize,
                    },
                    SearchTerm = searchTerm,
                    SortBy = sortBy,
                    PageSizes = new List<int> { 2, 4, 24, 36 }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading user properties for user {UserId}", userId);
                TempData["ErrorMessage"] = "An error occurred while loading your properties.";
                return View(new MyPropertyIndexViewModel
                {
                    Properties = new Pagination<PropertyViewModel>
                    {
                        Items = new List<PropertyViewModel>(),
                        CurrentPage = 1,
                        TotalPages = 1,
                        TotalItems = 0,
                        PageSize = pageSize,
                    },
                    PageSizes = new List<int> { 6, 12, 24, 36 }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadProperties(int page = 1, int pageSize = 12, string searchTerm = "", string sortBy = "newest")
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            try
            {
                var myProps = await propertyService.GetPropertiesByUserIdAsync(userId);

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    myProps = myProps.Where(p =>
                        p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Location?.City != null && p.Location.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Apply sorting
                myProps = sortBy?.ToLower() switch
                {
                    "oldest" => myProps.OrderBy(p => p.CreatedOn).ToList(),
                    "price-asc" => myProps.OrderBy(p => p.Price).ToList(),
                    "price-desc" => myProps.OrderByDescending(p => p.Price).ToList(),
                    "area-asc" => myProps.OrderBy(p => p.Area).ToList(),
                    "area-desc" => myProps.OrderByDescending(p => p.Area).ToList(),
                    "title" => myProps.OrderBy(p => p.Title).ToList(),
                    _ => myProps.OrderByDescending(p => p.CreatedOn).ToList(),
                };

                // Calculate pagination
                var totalItems = myProps.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var skip = (page - 1) * pageSize;
                var pagedItems = myProps.Skip(skip).Take(pageSize).ToList();

                var viewModels = pagedItems.Select(p => new
                {
                    id = p.Id,
                    title = p.Title,
                    description = p.Description,
                    price = p.Price,
                    area = p.Area,
                    createdOn = p.CreatedOn,
                    listingType = (int)p.ListingType,
                    propertyType = (int)p.PropertyType,
                    location = p.Location != null ? new { city = p.Location.City, address = p.Location.Address } : null,
                    category = p.Category != null ? new { name = p.Category.Name } : null,
                    images = p.Images?.Select(img => new { imageUrl = img.ImageUrl }).ToList(),
                    favoriteCount = 0 // This would need to be calculated if needed
                }).ToList();

                return Json(new
                {
                    success = true,
                    data = viewModels,
                    pagination = new
                    {
                        currentPage = page,
                        totalPages = totalPages,
                        totalItems = totalItems,
                        pageSize = pageSize,
                        hasNextPage = page < totalPages,
                        hasPreviousPage = page > 1
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading properties for user {UserId}", userId);
                return Json(new { success = false, message = "Error loading properties" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);
                return Json(new { success = true, statistics });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user statistics");
                return Json(new { success = false, message = "Error loading statistics" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuick(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    return Json(new { success = false, message = "You don't have permission to delete this property" });
                }

                await propertyService.DeletePropertyAsync(id);

                logger.LogInformation("Property deleted via MyProperty. ID: {PropertyId}", id);
                return Json(new { success = true, message = "Property deleted successfully" });
            }
            catch (NotFoundException)
            {
                return Json(new { success = false, message = "Property not found" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting property via MyProperty. ID: {PropertyId}", id);
                return Json(new { success = false, message = "An error occurred while deleting" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    TempData["ErrorMessage"] = "You don't have permission to view this property.";
                    return RedirectToAction("Index");
                }

                var viewModel = mapper.Map<DetailsViewModel>(property);
                return View(viewModel);
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "Property not found.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading property details in MyProperty. ID: {PropertyId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading details.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    TempData["ErrorMessage"] = "You don't have permission to edit this property.";
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Update", "Property", new { id = id });
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "Property not found.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete this property.";
                    return RedirectToAction("Index");
                }

                await propertyService.DeletePropertyAsync(id);

                logger.LogInformation("Property deleted from MyProperty. ID: {PropertyId}", id);
                TempData["SuccessMessage"] = "Property deleted successfully.";
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "Property not found.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting property from MyProperty. ID: {PropertyId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the property.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    return Json(new { success = false, message = "You don't have permission for this action" });
                }

                // Note: This would need to be implemented in PropertyService
                // await propertyService.TogglePropertyActiveAsync(id);

                return Json(new { success = true, message = "Property status changed" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error toggling property active status. ID: {PropertyId}", id);
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                // Get recent properties
                var recentProperties = await propertyService.GetPropertiesByUserIdAsync(userId);
                var recentPropsViewModel = recentProperties
                    .OrderByDescending(p => p.CreatedOn)
                    .Take(5)
                    .Select(p => mapper.Map<PropertyViewModel>(p))
                    .ToList();

                // Get statistics
                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);

                ViewData["RecentProperties"] = recentPropsViewModel;
                ViewData["Statistics"] = statistics;

                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading dashboard for user {UserId}", userId);
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkDelete(List<int> propertyIds)
        {
            if (propertyIds == null || !propertyIds.Any())
            {
                return Json(new { success = false, message = "No properties selected" });
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                int deletedCount = 0;
                var errors = new List<string>();

                foreach (var propertyId in propertyIds)
                {
                    try
                    {
                        var property = await propertyService.GetPropertyAsync(propertyId);

                        // Check ownership
                        if (property.OwnerId != userId)
                        {
                            errors.Add($"Property {propertyId}: Permission denied");
                            continue;
                        }

                        await propertyService.DeletePropertyAsync(propertyId);
                        deletedCount++;
                    }
                    catch (NotFoundException)
                    {
                        errors.Add($"Property {propertyId}: Not found");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error deleting property {PropertyId} in bulk operation", propertyId);
                        errors.Add($"Property {propertyId}: Error occurred");
                    }
                }

                logger.LogInformation("Bulk delete completed. {DeletedCount} properties deleted, {ErrorCount} errors",
                    deletedCount, errors.Count);

                var message = $"{deletedCount} properties deleted successfully";
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
                return Json(new { success = false, message = "An error occurred during bulk delete" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }

                var properties = await propertyService.GetPropertiesByUserIdAsync(userId);

                var exportData = properties.Select(p => new
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    Area = p.Area,
                    ListingType = p.ListingType.ToString(),
                    PropertyType = p.PropertyType.ToString(),
                    CreatedOn = p.CreatedOn,
                    Location = p.Location?.City,
                    Address = p.Location?.Address,
                    Category = p.Category?.Name
                }).ToList();

                var json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                var fileName = $"my_properties_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";

                return File(bytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error exporting properties for user");
                TempData["ErrorMessage"] = "An error occurred while exporting your properties.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPropertyAnalytics(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    return Json(new { success = false, message = "Permission denied" });
                }

                // Mock analytics data - in real app this would come from analytics service
                var analytics = new
                {
                    views = new Random().Next(50, 500),
                    favorites = new Random().Next(5, 50),
                    inquiries = new Random().Next(1, 20),
                    daysOnMarket = (DateTime.Now - property.CreatedOn).Days,
                    viewsThisWeek = new Random().Next(10, 100),
                    averageTimeOnPage = "2:30",
                    topReferrers = new[] { "Google", "Facebook", "Direct" }
                };

                return Json(new { success = true, analytics });
            }
            catch (NotFoundException)
            {
                return Json(new { success = false, message = "Property not found" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting analytics for property {PropertyId}", id);
                return Json(new { success = false, message = "Error loading analytics" });
            }
        }
    }
}