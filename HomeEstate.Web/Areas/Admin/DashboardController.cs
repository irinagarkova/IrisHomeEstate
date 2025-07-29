using AutoMapper;
using HomeEstate.Services.Core.Interfaces;
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
        private readonly IPropertyService propertyService; // Assuming you have this
        private readonly IMapper mapper;

        public DashboardController(IApplicationUserService accountService, IPropertyService propertyService, IMapper mapper)
        {
            this.applicationUserService = accountService;
            this.propertyService = propertyService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = new AdminDashboardViewModel
                {
                    TotalUsers = await applicationUserService.GetTotalUsersCount(),
                    TotalProperties = await propertyService.GetTotalPropertiesCount(),
                    RecentProperties = mapper.Map<List<PropertyViewModel>>(await propertyService.GetRecentProperties(5))
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load dashboard data.";
                return View(new AdminDashboardViewModel());
            }
        }
    }
}