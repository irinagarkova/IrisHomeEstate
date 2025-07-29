using AutoMapper;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IApplicationUserService applicationUserService;
        private readonly IMapper mapper;

        public UsersController(IApplicationUserService applicationUserService, IMapper mapper)
        {
            this.applicationUserService = applicationUserService;
            this.mapper = mapper;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var users = await applicationUserService.GetAllUsersAsync(page, pageSize);
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load users.";
                return View(new List<ApplicationUserViewModel>());
            }
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var user = await applicationUserService.GetApplicationUser(id);
                if (user == null)
                {
                    return NotFound();
                }

                var userViewModel = mapper.Map<ApplicationUserViewModel>(user);
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to find user.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                // Prevent admin from deleting themselves
                var currentUser = await applicationUserService.GetApplicationUser(User.Identity.Name);
                if (currentUser.Id == id)
                {
                    TempData["Error"] = "You cannot delete your own account.";
                    return RedirectToAction("Index");
                }

                await applicationUserService.DeleteUserAsync(id);
                TempData["Success"] = "User deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the user.";
            }

            return RedirectToAction("Index");
        }
    }
}