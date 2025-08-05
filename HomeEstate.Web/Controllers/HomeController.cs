using AutoMapper;
using HomeEstate.Models;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HomeEstate.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPropertyService propertyService;
        private readonly IMapper mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IPropertyService propertyService,
            IMapper mapper)
        {
            _logger = logger;
            this.propertyService = propertyService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Вземане на всички имоти
                var allProperties = await propertyService.GetAllPropertiesAsync(1, 20); 

                var featuredProperties = allProperties.Items
                    .Where(p => p.Images != null && p.Images.Any())
                    .TakeLast(3) 
                    .ToList();

                if (featuredProperties.Count < 3)
                {
                    var additionalProperties = allProperties.Items
                        .Where(p => !featuredProperties.Any(fp => fp.Id == p.Id)) 
                        .Take(3 - featuredProperties.Count)
                        .ToList();

                    featuredProperties.AddRange(additionalProperties);
                }

                // Мапване към ViewModel
                var featuredViewModels = featuredProperties.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

                return View(featuredViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading properties for home page");
                // При грешка връщаме празен списък
                return View(new List<PropertyViewModel>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Terms")]
        [Route("Home/Terms")]
        public IActionResult Terms()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == null)
            {
                return this.View("InternalServerError500");
            }

            switch (statusCode)
            {
                case 404:
                    return this.View("NotFound404");
                default:
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}