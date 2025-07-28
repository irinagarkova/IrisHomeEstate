using AutoMapper;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeEstate.Web.Controllers
{
    [Authorize]
    public class MyPropertyController : Controller
    {
        private readonly IPropertyService propertyService;
        private readonly IMapper mapper;
        private readonly ILogger<MyPropertyController> logger;

        public MyPropertyController(
            IPropertyService propertyService,
            IMapper mapper,
            ILogger<MyPropertyController> logger)
        {
            this.propertyService = propertyService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Показва имотите на текущия потребител
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var myProps = await propertyService.GetPropertiesByUserIdAsync(userId);
                var viewModel = myProps.Select(p => mapper.Map<PropertyViewModel>(p)).ToList();

                // Получаване на статистики
                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);
                ViewData["Statistics"] = statistics;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading user properties for user {UserId}", userId);
                TempData["ErrorMessage"] = "Възникна грешка при зареждането на вашите имоти.";
                return View(new List<PropertyViewModel>());
            }
        }

        /// <summary>
        /// AJAX метод за получаване на статистики за потребителя
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Json(new { success = false, message = "Потребителят не е намерен" });
                }

                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);
                return Json(new { success = true, statistics });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user statistics");
                return Json(new { success = false, message = "Възникна грешка при получаването на статистиките" });
            }
        }

        /// <summary>
        /// AJAX метод за бързо изтриване на имот
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteQuick(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    return Json(new { success = false, message = "Нямате права да изтриете този имот" });
                }

                await propertyService.DeletePropertyAsync(id);

                logger.LogInformation("Property deleted via MyProperty. ID: {PropertyId}", id);
                return Json(new { success = true, message = "Имотът е изтрит успешно" });
            }
            catch (NotFoundException)
            {
                return Json(new { success = false, message = "Имотът не е намерен" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting property via MyProperty. ID: {PropertyId}", id);
                return Json(new { success = false, message = "Възникна грешка при изтриването" });
            }
        }

        /// <summary>
        /// Показва подробности за имот (само за собственика)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    TempData["ErrorMessage"] = "Нямате права да видите този имот.";
                    return RedirectToAction("Index");
                }

                var viewModel = mapper.Map<DetailsViewModel>(property);
                return View(viewModel);
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "Имотът не е намерен.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading property details in MyProperty. ID: {PropertyId}", id);
                TempData["ErrorMessage"] = "Възникна грешка при зареждането на детайлите.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Показва формата за редактиране на имот
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    TempData["ErrorMessage"] = "Нямате права да редактирате този имот.";
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Update", "Property", new { id = id });
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "Имотът не е намерен.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Изтрива имот с redirect обратно към MyProperty
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    TempData["ErrorMessage"] = "Нямате права да изтриете този имот.";
                    return RedirectToAction("Index");
                }

                await propertyService.DeletePropertyAsync(id);

                logger.LogInformation("Property deleted from MyProperty. ID: {PropertyId}", id);
                TempData["SuccessMessage"] = "Имотът е изтрит успешно.";
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "Имотът не е намерен.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting property from MyProperty. ID: {PropertyId}", id);
                TempData["ErrorMessage"] = "Възникна грешка при изтриването на имота.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Активира/деактивира имот
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var property = await propertyService.GetPropertyAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (property.OwnerId != userId)
                {
                    return Json(new { success = false, message = "Нямате права за това действие" });
                }

                // Тук трябва да добавите метод в PropertyService за активиране/деактивиране
                // await propertyService.TogglePropertyActiveAsync(id);

                return Json(new { success = true, message = "Статусът на имота е променен" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error toggling property active status. ID: {PropertyId}", id);
                return Json(new { success = false, message = "Възникна грешка" });
            }
        }

        /// <summary>
        /// Dashboard с обобщена информация
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                // Получаване на последните 5 имота
                var recentProperties = await propertyService.GetPropertiesByUserIdAsync(userId);
                var recentPropsViewModel = recentProperties
                    .OrderByDescending(p => p.CreatedOn)
                    .Take(5)
                    .Select(p => mapper.Map<PropertyViewModel>(p))
                    .ToList();

                // Получаване на статистики
                var statistics = await propertyService.GetUserPropertyStatisticsAsync(userId);

                ViewData["RecentProperties"] = recentPropsViewModel;
                ViewData["Statistics"] = statistics;

                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading dashboard for user {UserId}", userId);
                TempData["ErrorMessage"] = "Възникна грешка при зареждането на dashboard-а.";
                return View();
            }
        }
    }
}