using AutoMapper;
using HomeEstate.Data.Models.Enum;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Services;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeEstate.Web.Controllers
{
    [Authorize]
    public class FavoritePropertyController : Controller
    {
        private readonly IFavoritePropertyService favoritePropertyService;
        private readonly IMapper mapper;
        private readonly IPropertyService propertyService;
        private readonly ILogger<FavoritePropertyController> logger;

        public FavoritePropertyController(
            IFavoritePropertyService favoritePropertyService,
            IMapper mapper,
            IPropertyService propertyService,
            ILogger<FavoritePropertyController> logger)
        {
            this.favoritePropertyService = favoritePropertyService;
            this.mapper = mapper;
            this.propertyService = propertyService;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string searchTerm = "", string sortBy = "newest", string category = "")
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 6) pageSize = 6;
            if (pageSize > 50) pageSize = 50;

            try
            {
                var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(User.Identity.Name);
                var propertyDtos = favorites.Select(fp => fp.Property).ToList();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    propertyDtos = propertyDtos.Where(p =>
                        p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Location?.City != null && p.Location.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(category))
                {
                    propertyDtos = propertyDtos.Where(p =>
                        p.Category != null && p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // Apply sorting
                propertyDtos = sortBy?.ToLower() switch
                {
                    "oldest" => propertyDtos.OrderBy(p => p.CreatedOn).ToList(),
                    "price-low" => propertyDtos.OrderBy(p => p.Price).ToList(),
                    "price-high" => propertyDtos.OrderByDescending(p => p.Price).ToList(),
                    "area-large" => propertyDtos.OrderByDescending(p => p.Area).ToList(),
                    "area-small" => propertyDtos.OrderBy(p => p.Area).ToList(),
                    "title" => propertyDtos.OrderBy(p => p.Title).ToList(),
                    _ => propertyDtos.OrderByDescending(p => p.CreatedOn).ToList(), // newest (default)
                };

                // Calculate pagination
                var totalItems = propertyDtos.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var skip = (page - 1) * pageSize;
                var pagedItems = propertyDtos.Skip(skip).Take(pageSize).ToList();

                var mapped = pagedItems.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

                // Set all as favorites since they're from favorites list
                foreach (var item in mapped)
                {
                    item.IsFavorite = true;
                    item.FavoriteCount = await favoritePropertyService.GetFavoriteCountForPropertyAsync(item.Id);
                }

                // Get unique categories for filter
                var allFavorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(User.Identity.Name);
                var categories = allFavorites
                    .Where(f => f.Property?.Category != null)
                    .Select(f => f.Property.Category.Name)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                // Create view model
                var viewModel = new FavoritePropertyIndexViewModel
                {
                    Properties = new Pagination<PropertyViewModel>
                    {
                        Items = mapped,
                        CurrentPage = page,
                        TotalPages = totalPages,
                        TotalItems = totalItems,
                        PageSize = pageSize,
                    },
                    SearchTerm = searchTerm,
                    SortBy = sortBy,
                    Category = category,
                    Categories = categories,
                    PageSizes = new List<int> { 2, 4, 6, 8 }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading favorite properties for user {UserName}", User.Identity.Name);
                TempData["ErrorMessage"] = "An error occurred while loading your favorite properties.";
                return View(new FavoritePropertyIndexViewModel
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
        public async Task<IActionResult> LoadFavorites(int page = 1, int pageSize = 12, string searchTerm = "", string sortBy = "newest", string category = "")
        {
            try
            {
                var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(User.Identity.Name);
                var propertyDtos = favorites.Select(fp => fp.Property).ToList();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    propertyDtos = propertyDtos.Where(p =>
                        p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Location?.City != null && p.Location.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(category))
                {
                    propertyDtos = propertyDtos.Where(p =>
                        p.Category != null && p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // Apply sorting
                propertyDtos = sortBy?.ToLower() switch
                {
                    "oldest" => propertyDtos.OrderBy(p => p.CreatedOn).ToList(),
                    "price-low" => propertyDtos.OrderBy(p => p.Price).ToList(),
                    "price-high" => propertyDtos.OrderByDescending(p => p.Price).ToList(),
                    "area-large" => propertyDtos.OrderByDescending(p => p.Area).ToList(),
                    "area-small" => propertyDtos.OrderBy(p => p.Area).ToList(),
                    "title" => propertyDtos.OrderBy(p => p.Title).ToList(),
                    _ => propertyDtos.OrderByDescending(p => p.CreatedOn).ToList(),
                };

                // Calculate pagination
                var totalItems = propertyDtos.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var skip = (page - 1) * pageSize;
                var pagedItems = propertyDtos.Skip(skip).Take(pageSize).ToList();

                var viewModels = new List<object>();
                foreach (var p in pagedItems)
                {
                    var favoriteCount = await favoritePropertyService.GetFavoriteCountForPropertyAsync(p.Id);
                    viewModels.Add(new
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
                        favoriteCount = favoriteCount,
                        isFavorite = true
                    });
                }

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
                logger.LogError(ex, "Error loading favorite properties for user {UserName}", User.Identity.Name);
                return Json(new { success = false, message = "Error loading favorite properties" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            try
            {
                await favoritePropertyService.AddPropertyToFavoriteAsync(id, User.Identity.Name);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding property {PropertyId} to favorites for user {UserName}", id, User.Identity.Name);
                return Json(new { success = false, message = "Error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                await favoritePropertyService.RemovePropertyFromFavoriteAsync(id, User.Identity.Name);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing property {PropertyId} from favorites for user {UserName}", id, User.Identity.Name);
                return BadRequest(new { success = false, message = "Error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteStats()
        {
            try
            {
                var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(User.Identity.Name);
                var properties = favorites.Select(f => f.Property).ToList();

                var stats = new
                {
                    totalFavorites = properties.Count,
                    totalValue = properties.Sum(p => p.Price),
                    averagePrice = properties.Any() ? properties.Average(p => p.Price) : 0,
                    totalArea = properties.Sum(p => p.Area),
                    averageArea = properties.Any() ? properties.Average(p => p.Area) : 0,
                    forSaleCount = properties.Count(p => p.ListingType == PropertyListingType.Sale || p.ListingType == PropertyListingType.Both),
                    forRentCount = properties.Count(p => p.ListingType == PropertyListingType.Rent || p.ListingType == PropertyListingType.Both),
                    byCategory = properties.GroupBy(p => p.Category?.Name ?? "Unknown")
                                          .Select(g => new { category = g.Key, count = g.Count() })
                                          .OrderByDescending(x => x.count)
                                          .ToList()
                };

                return Json(new { success = true, stats });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting favorite statistics for user {UserName}", User.Identity.Name);
                return Json(new { success = false, message = "Error loading statistics" });
            }
        }
    }
}