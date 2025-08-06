using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Data.Models;
using HomeEstate.Data.Models.Enum;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Services;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HomeEstate.Web.Controllers
{
    [Authorize]
    public class PropertyController : Controller
    {
        private readonly IPropertyService propertyService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFavoritePropertyService favoritePropertyService;

        public PropertyController(IPropertyService propertyService,
                                  IMapper mapper,
                                  UserManager<ApplicationUser> userManager,
                                  IFavoritePropertyService favoritePropertyService)
        {
            this.propertyService = propertyService;
            this.mapper = mapper;
            this.userManager = userManager;
            this.favoritePropertyService = favoritePropertyService;
        }

        [AllowAnonymous]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> Index(PropertySearchDto? searchCriteria = null, int page = 1, int pageSize = 5)
        {
            // Validate page parameters
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 100) pageSize = 100;
            if(searchCriteria.ListingType == null)
            {
                searchCriteria.ListingType = PropertyListingType.Sale;
            }
            searchCriteria ??= new PropertySearchDto();
            searchCriteria.Page = page;
            searchCriteria.PageSize = pageSize;

            Pagination<PropertyDto> propertyDtos;

            // Check if has search criteria
            bool hasSearchCriteria = !string.IsNullOrEmpty(searchCriteria.Location) ||
                          searchCriteria.CategoryId.HasValue ||
                          searchCriteria.MaxPrice.HasValue ||
                          searchCriteria.MaxRent.HasValue ||
                          searchCriteria.ListingType.HasValue ||
                          searchCriteria.Bedrooms.HasValue;

            if (hasSearchCriteria)
            {
                propertyDtos = await propertyService.SearchPropertiesAsync(searchCriteria);
                ViewData["SearchApplied"] = true;
                ViewData["SearchCriteria"] = searchCriteria;
            }
            else
            {
                propertyDtos = await propertyService.GetAllPropertiesAsync(page, pageSize);
                ViewData["SearchApplied"] = false;
            }

            var userName = User.Identity?.Name;
            var favoriteIds = new HashSet<int>();

            if (User.Identity != null && User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userName))
            {
                var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(userName);
                favoriteIds = favorites.Select(f => f.PropertyId).ToHashSet();
            }

            var mappedProperties = new List<PropertyViewModel>();

            foreach (var p in propertyDtos.Items)
            {
                var vm = mapper.Map<PropertyViewModel>(p);
                vm.IsFavorite = favoriteIds.Contains(p.Id);
                vm.FavoriteCount = await favoritePropertyService.GetFavoriteCountForPropertyAsync(p.Id);
                mappedProperties.Add(vm);
            }

            // Create pagination view model
            var viewModel = new PropertyIndexViewModel
            {
                Properties = new Pagination<PropertyViewModel>
                {
                    Items = mappedProperties,
                    CurrentPage = propertyDtos.CurrentPage,
                    TotalPages = propertyDtos.TotalPages,
                    TotalItems = propertyDtos.TotalItems,
                    PageSize = propertyDtos.PageSize,
                },
                SearchCriteria = searchCriteria,
                PageSizes = new List<int> { 5, 10, 25, 50, 100 }
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForSale(int page = 1, int pageSize = 10)
        {
            var searchCriteria = new PropertySearchDto
            {
                ListingType = PropertyListingType.Sale,
                Page = page,
                PageSize = pageSize
            };
            return await Index(searchCriteria);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForRent(int page = 1, int pageSize = 10)
        {
            var searchCriteria = new PropertySearchDto
            {
                ListingType = PropertyListingType.Rent,
                Page = page,
                PageSize = pageSize
            };
            return await Index(searchCriteria);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var model = new AddAndUpdatePropertyViewModel
            {
                ListingType = PropertyListingType.Sale,
                Locations = await getLocations()
            };
            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> getLocations()
        {
            var locations = await propertyService.GetAllLocations();

            return locations
                       .OrderBy(l => l.Id)
                       .Select(l => new SelectListItem
                       {
                           Value = l.Id.ToString(),
                           Text = l.City
                       })
                       .ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddAndUpdatePropertyViewModel model)
        {
            if (model.Images == null)
            {
                ModelState.AddModelError("Images", "Upload at least 1 image");
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ListingType == PropertyListingType.Rent || model.ListingType == PropertyListingType.Both)
            {
                if (!model.MonthlyRent.HasValue || model.MonthlyRent <= 0)
                {
                    ModelState.AddModelError("MonthlyRent", "Monthly rent is required for rental properties");
                    model.Locations = await getLocations();
                    return View(model);
                }
            }

            try
            {
                var imagePaths = new List<string>();
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                if (model.Images != null)
                {
                    foreach (var image in model.Images)
                    {
                        if (image != null && image.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(uploadFolder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            imagePaths.Add("/uploads/" + fileName);
                        }
                    }
                }

                var userId = userManager.GetUserId(User);
                var propertyDto = mapper.Map<PropertyDto>(model);
                propertyDto.OwnerId = userId;
                propertyDto.CreatedOn = DateTime.UtcNow;
                propertyDto.Images = imagePaths.Select(path => new PropertyImageDto
                {
                    ImageUrl = path
                }).ToList();

                await propertyService.CreatePropertyAsync(propertyDto);

                TempData["SuccessMessage"] = "Property created successfully!";
                return RedirectToAction("Index", "MyProperty");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the property.");
                model.Locations = await getLocations();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = userManager.GetUserId(User);

                if (property.OwnerId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var viewModel = mapper.Map<AddAndUpdatePropertyViewModel>(property);
                var locations = await propertyService.GetAllLocations();
                viewModel.Locations = locations.Select(l => new SelectListItem(l.City, l.Id.ToString())).ToList();
                return View(viewModel);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(AddAndUpdatePropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Locations = await getLocations();
                return View(model);
            }
            if (model.ListingType == PropertyListingType.Rent || model.ListingType == PropertyListingType.Both)
            {
                if (!model.MonthlyRent.HasValue || model.MonthlyRent <= 0)
                {
                    ModelState.AddModelError("MonthlyRent", "Monthly rent is required for rental properties");
                    model.Locations = await getLocations();
                    return View(model);
                }
            }
            try
            {
                var property = await propertyService.GetPropertyAsync(model.Id);
                var userId = userManager.GetUserId(User);

                if (property.OwnerId != userId)
                {
                    return Forbid();
                }

                var propertyDto = mapper.Map<PropertyDto>(model);
                propertyDto.OwnerId = userId;

                if (model.Images == null || !model.Images.Any())
                {
                    propertyDto.Images = property.Images;
                }
                else
                {
                    var imagePaths = new List<string>();
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    foreach (var image in model.Images)
                    {
                        if (image != null && image.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(uploadFolder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            imagePaths.Add("/uploads/" + fileName);
                        }
                    }

                    propertyDto.Images.AddRange(imagePaths.Select(path => new PropertyImageDto
                    {
                        ImageUrl = path
                    }).ToList());
                }

                await propertyService.UpdatePropertyAsync(propertyDto);

                TempData["SuccessMessage"] = "Property updated successfully!";
                return RedirectToAction("MyProperty");
            }
            catch (CustomValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                model.Locations = await getLocations();
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating.");
                model.Locations = await getLocations();
                return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var mappedProp = mapper.Map<DetailsViewModel>(property);

                var similarProperties = await propertyService.GetSimilarPropertiesAsync(id, 4);
                ViewBag.SimilarProperties = similarProperties.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

                return View(mappedProp);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the property.";
                return RedirectToAction("Index");
            };
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = userManager.GetUserId(User);

                if (property.OwnerId != userId)
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                await propertyService.DeletePropertyAsync(id);
                return Json(new { success = true });
            }
            catch (NotFoundException)
            {
                return Json(new { success = false, message = "Property not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await propertyService.GetPropertyAsync(id);
            var userId = userManager.GetUserId(User);
            if (property.OwnerId != userId)
            {
                return Forbid();
            }

            try
            {
                await propertyService.DeletePropertyAsync(id);
                return RedirectToAction("MyProperty");
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(PropertySearchDto searchCriteria)
        {
            var routeValues = new RouteValueDictionary();

            if (!string.IsNullOrEmpty(searchCriteria.Location))
                routeValues.Add("Location", searchCriteria.Location);

            if (searchCriteria.CategoryId.HasValue)
                routeValues.Add("CategoryId", searchCriteria.CategoryId.Value);

            if (searchCriteria.MaxPrice.HasValue)
                routeValues.Add("MaxPrice", searchCriteria.MaxPrice.Value);

            if (searchCriteria.MaxRent.HasValue)
                routeValues.Add("MaxRent", searchCriteria.MaxRent.Value);

            if (searchCriteria.ListingType.HasValue)
                routeValues.Add("ListingType", (int)searchCriteria.ListingType.Value);

            if (searchCriteria.Bedrooms.HasValue)
                routeValues.Add("Bedrooms", searchCriteria.Bedrooms.Value);

            if (!string.IsNullOrEmpty(searchCriteria.SortBy))
                routeValues.Add("SortBy", searchCriteria.SortBy);

            return RedirectToAction("Index", routeValues);
        }

        [HttpPost]
        public IActionResult ValidateRentalFields([FromBody] RentalValidationRequest request)
        {
            var errors = new List<string>();

            if (request.ListingType == PropertyListingType.Rent || request.ListingType == PropertyListingType.Both)
            {
                if (!request.MonthlyRent.HasValue || request.MonthlyRent <= 0)
                {
                    errors.Add("Monthly rent is required");
                }

                if (request.MinimumLeasePeriod.HasValue && (request.MinimumLeasePeriod < 1 || request.MinimumLeasePeriod > 60))
                {
                    errors.Add("Minimum lease period must be between 1 and 60 months");
                }

                if (request.SecurityDeposit.HasValue && request.SecurityDeposit < 0)
                {
                    errors.Add("Security deposit cannot be negative");
                }
            }

            return Json(new { isValid = !errors.Any(), errors = errors });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAjax([FromBody] PropertySearchDto searchCriteria, int page = 1, int pageSize = 5)
        {
            try
            {
                searchCriteria ??= new PropertySearchDto();
                searchCriteria.Page = page;
                searchCriteria.PageSize = pageSize;

                var propertyDtos = await propertyService.SearchPropertiesAsync(searchCriteria);

                var userName = User.Identity?.Name;
                var favoriteIds = new HashSet<int>();

                if (User.Identity != null && User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userName))
                {
                    var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(userName);
                    favoriteIds = favorites.Select(f => f.PropertyId).ToHashSet();
                }

                var mappedProperties = new List<object>();

                foreach (var p in propertyDtos.Items)
                {
                    var vm = mapper.Map<PropertyViewModel>(p);
                    vm.IsFavorite = favoriteIds.Contains(p.Id);
                    vm.FavoriteCount = await favoritePropertyService.GetFavoriteCountForPropertyAsync(p.Id);

                    var imagesList = new List<object>();
                    if (vm.Images != null && vm.Images.Any())
                    {
                        imagesList = vm.Images.Select(img => new
                        {
                            imageUrl = img.ImageUrl
                        }).Cast<object>().ToList();
                    }

                    mappedProperties.Add(new
                    {
                        id = vm.Id,
                        title = vm.Title,
                        description = vm.Description,
                        price = vm.Price,
                        area = vm.Area,
                        createdOn = vm.CreatedOn,
                        favoriteCount = vm.FavoriteCount,
                        isFavorite = vm.IsFavorite,
                        listingType = (int)vm.ListingType,
                        propertyType = (int)vm.PropertyType,
                        location = vm.Location != null ? new
                        {
                            city = vm.Location.City,
                            address = vm.Location.Address
                        } : null,
                        category = vm.Category != null ? new
                        {
                            name = vm.Category.Name
                        } : null,
                        images = imagesList
                    });
                }

                return Json(new
                {
                    success = true,
                    properties = mappedProperties,
                    totalCount = propertyDtos.TotalItems,
                    currentPage = propertyDtos.CurrentPage,
                    totalPages = propertyDtos.TotalPages,
                    hasNextPage = propertyDtos.HasNextPage,
                    hasPreviousPage = propertyDtos.HasPreviousPage,
                    hasSearchCriteria = !string.IsNullOrEmpty(searchCriteria.Location) ||
                                        searchCriteria.CategoryId.HasValue ||
                                        searchCriteria.MaxPrice.HasValue ||
                                        searchCriteria.MaxRent.HasValue ||
                                        searchCriteria.ListingType.HasValue ||
                                        searchCriteria.Bedrooms.HasValue
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while searching properties.",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestSearch(string location = "", int? categoryId = null)
        {
            try
            {
                var searchCriteria = new PropertySearchDto
                {
                    Location = location,
                    CategoryId = categoryId
                };

                return await SearchAjax(searchCriteria);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuickSearch(string location, int? categoryId, decimal? maxPrice, string sortBy = "newest")
        {
            var searchCriteria = new PropertySearchDto
            {
                Location = location,
                CategoryId = categoryId,
                MaxPrice = maxPrice,
                SortBy = sortBy
            };

            return await SearchAjax(searchCriteria);
        }
    }

    // Supporting classes
    public class RentalValidationRequest
    {
        public PropertyListingType ListingType { get; set; }
        public decimal? MonthlyRent { get; set; }
        public int? MinimumLeasePeriod { get; set; }
        public decimal? SecurityDeposit { get; set; }
    }
}