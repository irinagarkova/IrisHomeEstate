using AutoMapper;
using HomeEstate.Data;
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

        // Updated Index action to support both sale and rent
        [AllowAnonymous]
        public async Task<IActionResult> Index(PropertyListingType? type = null)
        {
            ICollection<PropertyDto> propertyDtos;

            if (type.HasValue)
            {
                propertyDtos = await propertyService.GetPropertiesByTypeAsync(type.Value);
                ViewData["ListingType"] = type.Value;
            }
            else
            {
                propertyDtos = await propertyService.GetAllPropertiesAsync();
                ViewData["ListingType"] = "All";
            }

            var userName = User.Identity?.Name;
            var favoriteIds = new HashSet<int>();

            if (User.Identity != null && User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userName))
            {
                var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(userName);
                favoriteIds = favorites.Select(f => f.PropertyId).ToHashSet();
            }

            var mappedProperties = new List<PropertyViewModel>();

            foreach (var p in propertyDtos)
            {
                var vm = mapper.Map<PropertyViewModel>(p);
                vm.IsFavorite = favoriteIds.Contains(p.Id);
                vm.FavoriteCount = await favoritePropertyService.GetFavoriteCountForPropertyAsync(p.Id);
                mappedProperties.Add(vm);
            }

            return View(mappedProperties);
        }

        // New action for properties for sale
        [AllowAnonymous]
        public async Task<IActionResult> ForSale()
        {
            return await Index(PropertyListingType.Sale);
        }

        // New action for properties for rent
        [AllowAnonymous]
        public async Task<IActionResult> ForRent()
        {
            return await Index(PropertyListingType.Rent);
        }

        // Updated AJAX Search Action
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] PropertySearchDto searchCriteria)
        {
            try
            {
                var properties = await propertyService.SearchPropertiesAsync(searchCriteria);

                // Get favorite status if user is authenticated
                var favoriteIds = new HashSet<int>();
                if (User.Identity?.IsAuthenticated == true)
                {
                    var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(User.Identity.Name);
                    favoriteIds = favorites.Select(f => f.PropertyId).ToHashSet();
                }

                var propertyResults = properties.Select(p => new
                {
                    id = p.Id,
                    title = p.Title,
                    description = p.Description,
                    price = p.Price,
                    monthlyRent = p.MonthlyRent,
                    listingType = p.ListingType.ToString(),
                    area = p.Area,
                    location = p.Location?.City ?? "Unknown",
                    category = p.Category?.Name ?? "Unknown",
                    images = p.Images.Select(i => i.ImageUrl).ToList(),
                    createdOn = p.CreatedOn,
                    isFavorite = favoriteIds.Contains(p.Id),
                    favoriteCount = favoritePropertyService.GetFavoriteCountForPropertyAsync(p.Id).Result,
                    petsAllowed = p.PetsAllowed,
                    isFurnished = p.IsFurnished,
                    availableFrom = p.AvailableFrom,
                    securityDeposit = p.SecurityDeposit,
                    minimumLeasePeriod = p.MinimumLeasePeriod
                }).ToList();

                return Json(new { success = true, properties = propertyResults });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Updated Add action
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
        public async Task<IActionResult> Add(AddAndUpdatePropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
				model.Locations = GetLocations();
				return View(model); 
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
                propertyDto.Images = imagePaths.Select(path => new PropertyImageDto
                {
                    ImageUrl = path
                }).ToList();

                await propertyService.CreatePropertyAsync(propertyDto);
                return RedirectToAction("MyProperty");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the property.");
                return View(model);
            }
        }

        // Updated Update action
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
                return View(model);
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

                await propertyService.UpdatePropertyAsync(propertyDto);
                return RedirectToAction("MyProperty");
            }
            catch (CustomValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating.");
                return View(model);
            }
        }

        // New AJAX action to get property statistics
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);

                return Json(new { success = true, statistics });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Updated MyProperty to show statistics
        [HttpGet]
        public async Task<IActionResult> MyProperty()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var myProps = await propertyService.GetPropertiesByUserIdAsync(userId);
            var viewModel = myProps.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

            // Get statistics
            var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);
            ViewData["Statistics"] = statistics;

            return View(viewModel);
        }

        // Existing actions remain the same...
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


    }

}
