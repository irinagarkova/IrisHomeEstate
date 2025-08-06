// Create this as a new file: Middleware/AuthenticationRefreshMiddleware.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using HomeEstate.Models;
using System.Security.Claims;

namespace HomeEstate.Web.Middleware
{
    public class AuthenticationRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationRefreshMiddleware> _logger;

        public AuthenticationRefreshMiddleware(RequestDelegate next, ILogger<AuthenticationRefreshMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            // Only process authenticated requests
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Check if this is a fresh external login that needs claims refresh
                var needsRefresh = context.Request.Cookies.ContainsKey("ExternalLoginRefresh");

                if (needsRefresh)
                {
                    try
                    {
                        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            var user = await userManager.FindByIdAsync(userId);
                            if (user != null)
                            {
                                // Refresh the sign-in to ensure all claims are loaded
                                await signInManager.RefreshSignInAsync(user);
                                _logger.LogInformation("Refreshed authentication for user {UserId}", userId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refreshing authentication");
                    }

                    // Remove the refresh cookie
                    context.Response.Cookies.Delete("ExternalLoginRefresh");
                }
            }

            await _next(context);
        }
    }
}