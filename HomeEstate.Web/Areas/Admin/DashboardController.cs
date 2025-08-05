using AutoMapper;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Areas.Admin.Models;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IApplicationUserService applicationUserService;
        private readonly IPropertyService propertyService;
        private readonly IMapper mapper;
        private readonly ILogger<DashboardController> logger;

        public DashboardController(
            IApplicationUserService applicationUserService,
            IPropertyService propertyService,
            IMapper mapper,
            ILogger<DashboardController> logger)
        {
            this.applicationUserService = applicationUserService;
            this.propertyService = propertyService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = new AdminDashboardViewModel
                {
                    TotalUsers = await applicationUserService.GetTotalUsersCount(),
                    TotalProperties = await propertyService.GetTotalPropertiesCount(),
                    RecentUsers = mapper.Map<List<ApplicationUserViewModel>>(await applicationUserService.GetRecentUsers(5)),
                    RecentProperties = mapper.Map<List<PropertyViewModel>>(await propertyService.GetRecentProperties(5))
                };

                // Calculate monthly stats (you might want to add these methods to your services)
                dashboardData.NewUsersThisMonth = 0; // Implement this in ApplicationUserService
                dashboardData.NewPropertiesThisMonth = 0; // Implement this in PropertyService

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = "Unable to load dashboard data.";
                return View(new AdminDashboardViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    totalUsers = await applicationUserService.GetTotalUsersCount(),
                    totalProperties = await propertyService.GetTotalPropertiesCount(),
                    // Add more stats as needed
                };

                return Json(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting dashboard stats");
                return Json(new { success = false, message = "Error loading stats" });
            }
        }
    }
}