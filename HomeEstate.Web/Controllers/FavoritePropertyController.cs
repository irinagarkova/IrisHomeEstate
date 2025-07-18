using AutoMapper;
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

        public FavoritePropertyController(IFavoritePropertyService favoritePropertyService, IMapper mapper, IPropertyService propertyService)
		{
			this.favoritePropertyService = favoritePropertyService;
			this.mapper = mapper;
            this.propertyService = propertyService;
        }
		public async Task<IActionResult> Index()
		{

            var favorites = await favoritePropertyService.GetAllFavoritePropertiesAsync(User.Identity.Name);
            var propertyDtos = favorites.Select(fp => fp.Property).ToList();
            var mapped = propertyDtos.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

            return View(mapped);
        }

		[HttpPost]
		public  async Task<IActionResult>Add(int id)
		{
			await favoritePropertyService.AddPropertyToFavoriteAsync(id, User.Identity.Name);
			return RedirectToAction("Index");
			
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            await favoritePropertyService.RemovePropertyFromFavoriteAsync(id, User.Identity.Name);
            return RedirectToAction("Index");
        }

    }
}
