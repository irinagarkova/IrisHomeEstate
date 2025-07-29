using AutoMapper;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IApplicationUserService accountService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public AccountController(IApplicationUserService applicationUserService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            this.accountService = applicationUserService;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await accountService.GetApplicationUser(User.Identity.Name);
                var userViewModel = mapper.Map<ApplicationUser>(user);
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public async Task<IActionResult> Edit()
        {
            try
            {
                var user = await accountService.GetApplicationUser(User.Identity.Name);
                var userViewModel = mapper.Map<ApplicationUserViewModel>(user);
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load profile for editing.";
                return RedirectToAction("Index");
            }
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = mapper.Map<ApplicationUserDto>(model);
                await accountService.UpdateApplicationUser(user);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Index");
               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating your profile.");
                return View(model);
            }
        }
    }
}
