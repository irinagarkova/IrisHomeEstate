using HomeEstate.Services.Core.Interfaces;
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

        public PropertiesController(IPropertyService propertyService)
        {
            this.propertyService = propertyService;
        }

        // GET: Admin/Properties
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var properties = await propertyService.GetAllPropertiesAsync(page, pageSize);
                return View(properties);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load properties.";
                return View(new List<PropertyViewModel>());
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
                    return NotFound();
                }

                return View(property);
            }
            catch (Exception ex)
            {
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
                TempData["Success"] = "Property deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the property.";
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Properties/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                if (property == null)
                {
                    return NotFound();
                }

                return View(property);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load property details.";
                return RedirectToAction("Index");
            }
        }
    }
}