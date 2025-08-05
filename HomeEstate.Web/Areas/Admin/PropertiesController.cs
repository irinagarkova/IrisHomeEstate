using AutoMapper;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Areas.Admin.Models;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PropertiesController : Controller
    {
        private readonly IPropertyService propertyService;
        private readonly IMapper mapper;
        private readonly ILogger<PropertiesController> logger;

        public PropertiesController(
            IPropertyService propertyService,
            IMapper mapper,
            ILogger<PropertiesController> logger)
        {
            this.propertyService = propertyService;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: Admin/Properties
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = "", string sortBy = "newest")
        {
            try
            {
                // Validate pagination parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(Math.Max(1, pageSize), 50); // Max 50 items per page

                var properties = await propertyService.GetAllPropertiesAsync(page, pageSize);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchCriteria = new PropertySearchDto
                    {
                        Location = searchTerm,
                        SortBy = sortBy
                    };

                    var searchResults = await propertyService.SearchPropertiesAsync(searchCriteria);

                    // Convert search results to paginated format
                    var totalPages = (int)Math.Ceiling((double)searchResults.Items.Count / pageSize);
                    var pagedItems = searchResults.Items
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var dict = new Dictionary<int, List<PropertyDto>>();

                    for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
                    {
                        var pageItems = searchResults.Items
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        dict[pageNumber] = pageItems;
                    }
                    properties = new Pagination<PropertyDto>
                    {
                        Items = pagedItems,
                        Properties = dict,
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = searchResults.Items.Count,
                        TotalPages = totalPages
                    };
                }

                var viewModel = new AdminPropertiesViewModel
                {
                    Properties = properties,
                    SearchTerm = searchTerm,
                    SortBy = sortBy,
                    PageSizes = new[] { 5, 10, 25, 50 }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading properties for admin");
                TempData["Error"] = "Unable to load properties.";
                return View(new AdminPropertiesViewModel());
            }
        }

        // AJAX endpoint for loading properties
        [HttpGet]
        public async Task<IActionResult> LoadProperties(int page = 1, int pageSize = 10, string searchTerm = "", string sortBy = "newest")
        {
            try
            {
                page = Math.Max(1, page);
                pageSize = Math.Min(Math.Max(1, pageSize), 50);

                var properties = await propertyService.GetAllPropertiesAsync(page, pageSize);

                //if (!string.IsNullOrEmpty(searchTerm))
                //{
                    var searchCriteria = new PropertySearchDto
                    {
                        Location = searchTerm,
                        SortBy = sortBy
                    };

                    var searchResults = await propertyService.SearchPropertiesAsync(searchCriteria);

                    var totalPages = (int)Math.Ceiling((double)searchResults.Items.Count / pageSize);
                    var pagedItems = searchResults.Items
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                   

                    properties = new Pagination<PropertyDto>
                    {
                        Items = pagedItems,
                        //Properties = dict,
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = searchResults.Items.Count,
                        TotalPages = totalPages
                    };
                //}

                var propertiesViewModel = properties.Items.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();
                
                var dict = new Dictionary<int, List<PropertyViewModel>>();
                for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
                {
                    var pageItems = propertiesViewModel
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    dict[pageNumber] = pageItems;
                }
                return Json(new
                {
                    success = true,
                    data = propertiesViewModel,
                    dict = dict,
                    pagination = new
                    {
                        currentPage = properties.CurrentPage,
                        totalPages = properties.TotalPages,
                        totalItems = properties.TotalItems,
                        pageSize = properties.PageSize,
                        hasPreviousPage = properties.HasPreviousPage,
                        hasNextPage = properties.HasNextPage
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading properties via AJAX");
                return Json(new { success = false, message = "Error loading properties" });
            }
        }

        // GET: Admin/Properties/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                if (property == null)
                {
                    TempData["Error"] = "Property not found.";
                    return RedirectToAction("Index");
                }

                var viewModel = mapper.Map<DetailsViewModel>(property);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading property details for admin. ID: {PropertyId}", id);
                TempData["Error"] = "Unable to load property details.";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Properties/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                if (property == null)
                {
                    TempData["Error"] = "Property not found.";
                    return RedirectToAction("Index");
                }

                var viewModel = mapper.Map<PropertyViewModel>(property);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading property for deletion. ID: {PropertyId}", id);
                TempData["Error"] = "Unable to find property.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await propertyService.DeletePropertyAsync(id);
                logger.LogInformation("Property deleted by admin. ID: {PropertyId}", id);
                TempData["Success"] = "Property deleted successfully.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting property. ID: {PropertyId}", id);
                TempData["Error"] = "An error occurred while deleting the property.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                await propertyService.DeletePropertyAsync(id);
                logger.LogInformation("Property deleted via AJAX by admin. ID: {PropertyId}", id);

                return Json(new
                {
                    success = true,
                    message = "Property deleted successfully"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting property via AJAX. ID: {PropertyId}", id);
                return Json(new
                {
                    success = false,
                    message = "Error deleting property"
                });
            }
        }

        // Bulk operations
        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromBody] int[] propertyIds)
        {
            try
            {
                if (propertyIds == null || propertyIds.Length == 0)
                {
                    return Json(new { success = false, message = "No properties selected" });
                }

                var deletedCount = 0;
                foreach (var id in propertyIds)
                {
                    try
                    {
                        await propertyService.DeletePropertyAsync(id);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error deleting property in bulk operation. ID: {PropertyId}", id);
                    }
                }

                logger.LogInformation("Bulk delete operation completed. Deleted {Count} properties", deletedCount);

                return Json(new
                {
                    success = true,
                    message = $"Successfully deleted {deletedCount} properties"
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