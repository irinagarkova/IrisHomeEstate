using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HomeEstate.Web.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService propertyService;
        private readonly IMapper mapper;

        public PropertyController(IPropertyService propertyService, IMapper mapper)
        {
            this.propertyService = propertyService;
            this.mapper = mapper;
        }

        //BasovClass moita figura = new Triugulnik

        //figura.SmetniPlo6t = \
        //figura = new Pravoulnik.
        //figura.SmmetniPlo6t



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

            var mapperToDtO = mapper.Map<PropertyDto>(model);
            await propertyService.CreatePropertyAsync(mapperToDtO);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int id)
        {
            var newDto = await propertyService.GetPropertyAsync(id);
            var propertyToUpdate = mapper.Map<AddAndUpdatePropertyViewModel>(newDto);
            return View(propertyToUpdate);
            
        }
        [HttpPost]
        public async Task<IActionResult> Update(AddAndUpdatePropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await propertyService.UpdatePropertyAsync(mapper.Map<PropertyDto>(model));


            return RedirectToAction("Index");
        }


    }

}
