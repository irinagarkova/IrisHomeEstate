using AutoMapper;
using HomeEstate.Models;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly IMapper mapper;
        private readonly IApplicationUserService userService;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager,
							  IMapper mapper,
							  IApplicationUserService userService)
        {
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.roleManager = roleManager;
			this.mapper = mapper;
            this.userService = userService;
        }

		public IActionResult Register()
		{
			if(User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = mapper.Map<ApplicationUser>(model);
			var result = await userManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				await userManager.AddToRoleAsync(user, "User");
				await signInManager.SignInAsync(user, isPersistent: true);
				return RedirectToAction("Index", "Home");
			}
			
			foreach(var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);

		}
		public IActionResult Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await signInManager.PasswordSignInAsync(model.Email,
																 model.Password,
																 model.RememberMe,
																 false);
			if (result.Succeeded)
			{
				return Json(new {Success = true});
			}
				
			ModelState.AddModelError(string.Empty, "Invalid credentials");
			return View(model);
                                                     
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
    }
}
