using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Controllers
{
    public class PropertyController : Controller
    {
       private readonly IPropertyService propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            this.propertyService = propertyService;
        }
        public async Task<IActionResult> Index()
        {
            //var prop = await propertyService.GetPropertyAsync(1);
            var property = new Property
            {
                Id = 1,
                Price = 234m,
                Title = "Maze"
            };
            
            return View(property);
        }

    }
    
}
