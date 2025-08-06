using AutoMapper;
using HomeEstate.Models;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Services;
using HomeEstate.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace HomeEstate.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly IApplicationUserService userService;
        private readonly IEmailService emailService;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager,
                              IMapper mapper,
                              IApplicationUserService userService,
                              IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.userService = userService;
            this.emailService = emailService;
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
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
                try
                {
                    await emailService.SendWelcomeEmailAsync(user.Email, user.UserName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Json(new {success = false, error = "Invalid credentials" });
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                // Email not confirmed - show message
                ModelState.AddModelError(string.Empty, "You must confirm your email before logging in.");
                return View(model);
            }


            var result = await signInManager.PasswordSignInAsync(model.Email,
                                                                 model.Password,
                                                                 model.RememberMe,
                                                                 false);
            if (result.Succeeded)
            {
                return Json(new { Success = true });
            }
            return Json(new { success = false, error = "Invalid credentials" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPassword(string token = null, string email = null)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "A code must be supplied for password reset.");
                return View("Error");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if (model.NewPassword != model.ConfirmPassword)
            {
                return Json(new { success = false, errors = new[] { "Passwords must match" } });
            }
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = result.Errors });
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Auth",
                new { token = token, email = model.Email }, Request.Scheme);

            // Send email
            try
            {
                var emailSent = await emailService.SendPasswordResetEmailAsync(model.Email, callbackUrl);

                if (!emailSent)
                {
                    // Log error but still show confirmation page for security
                    TempData["EmailError"] = "There was an issue sending the email. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                // Log error but still show confirmation page for security
                TempData["EmailError"] = "There was an issue sending the email. Please try again later.";
            }

            // For testing purposes - remove in production
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                TempData["ResetPasswordUrl"] = callbackUrl;
                TempData["ResetEmail"] = model.Email;
            }

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GoogleLogin(string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        // Google Callback Handler
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                TempData["Error"] = $"Error from Google: {remoteError}";
                return RedirectToAction("Login", "Home");
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Error"] = "Error loading Google login information.";
                return RedirectToAction("Login", "Home");
            }

            // Try to sign in with existing external login
            var result = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (result.Succeeded)
            {
                var existingUser = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingUser != null)
                {
                    await UpdateUserClaimsFromGoogle(existingUser, info);
                    await signInManager.SignOutAsync();
                    await Task.Delay(100);
                    await signInManager.SignInAsync(existingUser, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (email != null)
            {
                var applicationUser = await userManager.FindByEmailAsync(email);
                if (applicationUser != null)
                {
                    await userManager.AddLoginAsync(applicationUser, info);
                    await UpdateUserClaimsFromGoogle(applicationUser, info);
                    await signInManager.SignInAsync(applicationUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    await userManager.AddLoginAsync(user, info);
                    await userManager.AddToRoleAsync(user, "User");
                    await UpdateUserClaimsFromGoogle(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["Error"] = "Unable to load user information from Google.";
            return RedirectToAction("Login", "Home");
        }

        private async Task UpdateUserClaimsFromGoogle(ApplicationUser user, ExternalLoginInfo info)
        {
            var existingClaims = await userManager.GetClaimsAsync(user);

            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var picture = info.Principal.FindFirstValue("picture");
            var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var surname = info.Principal.FindFirstValue(ClaimTypes.Surname);

            await UpdateClaim(user, existingClaims, ClaimTypes.Name, name);
            await UpdateClaim(user, existingClaims, "picture", picture);
            await UpdateClaim(user, existingClaims, ClaimTypes.GivenName, givenName);
            await UpdateClaim(user, existingClaims, ClaimTypes.Surname, surname);
        }

        private async Task UpdateClaim(ApplicationUser user, IList<Claim> existingClaims, string claimType, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return;

            var existingClaim = existingClaims.FirstOrDefault(c => c.Type == claimType);

            if (existingClaim != null && existingClaim.Value != newValue)
            {
                await userManager.RemoveClaimAsync(user, existingClaim);
                await userManager.AddClaimAsync(user, new Claim(claimType, newValue));
            }
            else if (existingClaim == null)
            {
                await userManager.AddClaimAsync(user, new Claim(claimType, newValue));
            }
        }
        private IActionResult RedirectToLocal(string returnUrl)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
        }
    } 