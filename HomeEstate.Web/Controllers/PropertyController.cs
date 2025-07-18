using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Services;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public PropertyController(IPropertyService propertyService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            this.propertyService = propertyService;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            var propertyDtos = await propertyService.GetAllPropertiesAsync();
            var mappedProperties = propertyDtos.Select(x => mapper.Map<PropertyViewModel>(x)).ToList();
           
            return View(mappedProperties);

        }
   
        public IActionResult Add()
        {
            return View(new AddAndUpdatePropertyViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAndUpdatePropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
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

                // ✅ Добавете снимките!
                propertyDto.Images = imagePaths.Select(path => new PropertyImageDto
                {
                    ImageUrl = path
                }).ToList();

                await propertyService.CreatePropertyAsync(propertyDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Възникна грешка при създаването на обявата.");
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
                propertyDto.OwnerId = userId; // Запазете OwnerId!

                await propertyService.UpdatePropertyAsync(propertyDto);
                return RedirectToAction("Index");
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
                ModelState.AddModelError("", "Възникна грешка при обновяването.");
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
       
        [HttpGet]
        public async Task<IActionResult> MyProperties()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var myProps = await propertyService.GetPropertiesByUserIdAsync(userId);
            var viewModel = myProps.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

            return View(viewModel);
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
                return RedirectToAction("Index");
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }


    }

}
