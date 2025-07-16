using AutoMapper;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Services;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeEstate.Web.Controllers
{
	public class FavoritePropertyController : Controller
	{
		private readonly IFavoritePropertyService favoritePropertyService;
		private readonly IMapper mapper;

		public FavoritePropertyController(IFavoritePropertyService favoritePropertyService, IMapper mapper)
		{
			this.favoritePropertyService = favoritePropertyService;
			this.mapper = mapper;
		}
		public async Task<IActionResult> Index()
		{
			var prop = await favoritePropertyService.GetAllFavoritePropertiesAsync();
			var mapp = prop.Select(p => mapper.Map<FavoritePropertyViewModel>(p)).ToList();
			return View(mapp);
		}

		public  async Task<IActionResult>Add(FavoritePropertyViewModel model)
		{
			return View();
		}

	}
}
