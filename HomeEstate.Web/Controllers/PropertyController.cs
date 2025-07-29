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
        public async Task<IActionResult> Index(PropertySearchDto? searchCriteria = null)
        {

            // Ако searchCriteria е null, създайте празен
            searchCriteria ??= new PropertySearchDto();
            PaginatedDto<PropertyDto> propertyDtos;

            // Ако има search критерии
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
                ViewData["ResultsCount"] = propertyDtos.TotalItems;
            }
            else
            {
                propertyDtos = await propertyService.GetAllPropertiesAsync(1,10);
                ViewData["SearchApplied"] = false;
                ViewData["ResultsCount"] = propertyDtos.TotalItems;
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

            return View(mappedProperties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForSale()
        {
            var searchCriteria = new PropertySearchDto
            {
                ListingType = PropertyListingType.Sale
            };
            return await Index(searchCriteria);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForRent()
        {
            var searchCriteria = new PropertySearchDto
            {
                ListingType = PropertyListingType.Rent
            };
            return await Index(searchCriteria);
        }

       
        public IActionResult Add()
        {
            var model = new AddAndUpdatePropertyViewModel
            {
                ListingType = PropertyListingType.Sale,
                Locations = GetLocations() // Default
            };
            return View(model);
        }
        private IEnumerable<SelectListItem> GetLocations()
        {
            var locations = propertyService.GetAllLocations();

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
            if (!ModelState.IsValid)
            {
                model.Locations = GetLocations();
                return View(model);
            }
            if (model.ListingType == PropertyListingType.Rent || model.ListingType == PropertyListingType.Both)
            {
                if (!model.MonthlyRent.HasValue || model.MonthlyRent <= 0)
                {
                    ModelState.AddModelError("MonthlyRent", "Месечният наем е задължителен за имоти под наем");
                    model.Locations = GetLocations();
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

                TempData["SuccessMessage"] = "Имотът е създаден успешно!";
                return RedirectToAction("Index", "MyProperty");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the property.");
                model.Locations = GetLocations();
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

                if (property.OwnerId != userId)
                {
                    return Forbid();
                }

                var viewModel = mapper.Map<AddAndUpdatePropertyViewModel>(property);
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
                model.Locations = GetLocations();
                return View(model);
            }
            if (model.ListingType == PropertyListingType.Rent || model.ListingType == PropertyListingType.Both)
            {
                if (!model.MonthlyRent.HasValue || model.MonthlyRent <= 0)
                {
                    ModelState.AddModelError("MonthlyRent", "Месечният наем е задължителен за имоти под наем");
                    model.Locations = GetLocations();
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

                // Запазване на съществуващите снимки ако няма нови
                if (model.Images == null || !model.Images.Any())
                {
                    propertyDto.Images = property.Images;
                }
                else
                {
                    // Качване на нови снимки
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

                    propertyDto.Images = imagePaths.Select(path => new PropertyImageDto
                    {
                        ImageUrl = path
                    }).ToList();
                }

                await propertyService.UpdatePropertyAsync(propertyDto);

                TempData["SuccessMessage"] = "Имотът е обновен успешно!";
                return RedirectToAction("MyProperty");
            }
            catch (CustomValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                model.Locations = GetLocations();
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Възникна грешка при обновяването.");
                model.Locations = GetLocations();
                return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var property = await propertyService.GetPropertyAsync(id);
            var mappedProp = mapper.Map<DetailsViewModel>(property);

            return View(mappedProp);
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
        public async Task<IActionResult> Search(SearchViewModel model)
        {
            var search = mapper.Map<SearchPropertyDto>(model);
            var allprop = await propertyService.GetAllPropertiesAsync(search);

            return Json(new { Success = true, Properties = allprop });
        }

        [HttpPost]
        public IActionResult ValidateRentalFields([FromBody] RentalValidationRequest request)
        {
            var errors = new List<string>();

            if (request.ListingType == PropertyListingType.Rent || request.ListingType == PropertyListingType.Both)
            {
                if (!request.MonthlyRent.HasValue || request.MonthlyRent <= 0)
                {
                    errors.Add("Месечният наем е задължителен");
                }

                if (request.MinimumLeasePeriod.HasValue && (request.MinimumLeasePeriod < 1 || request.MinimumLeasePeriod > 60))
                {
                    errors.Add("Минималният срок трябва да бъде между 1 и 60 месеца");
                }

                if (request.SecurityDeposit.HasValue && request.SecurityDeposit < 0)
                {
                    errors.Add("Депозитът не може да бъде отрицателен");
                }
            }

            return Json(new { isValid = !errors.Any(), errors = errors });
        }
    }

}
