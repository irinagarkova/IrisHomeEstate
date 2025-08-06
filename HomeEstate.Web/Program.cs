using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Mappings;
using HomeEstate.Services.Core.Services;
using HomeEstate.Web.Mappings;
using HomeEstate.Web.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeEstate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<HomeEstateDbContext>(options =>
                
                options.UseSqlServer(builder.Configuration.GetConnectionString("Local") ?? Environment.GetEnvironmentVariable("DefaultConnection")));


            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<PropertyMappingProfile>();
                cfg.AddProfile<PropertyWebMappingProfile>();
            });

            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped<IFavoritePropertyService, FavoritePropertyService>();
            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services
              .AddIdentity<ApplicationUser, IdentityRole>(options =>
              {
                  options.SignIn.RequireConfirmedEmail = false;
                  options.SignIn.RequireConfirmedAccount = false;
                  options.SignIn.RequireConfirmedPhoneNumber = false;

                  options.Password.RequiredLength = 3;
                  options.Password.RequireDigit = false;
                  options.Password.RequireNonAlphanumeric = false;
                  options.Password.RequireLowercase = false;
                  options.Password.RequireUppercase = false;
                  options.Password.RequiredUniqueChars = 0;
              })
              .AddEntityFrameworkStores<HomeEstateDbContext>()
              .AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var clientId = builder.Configuration["Authentication:Google:ClientId"] ?? Environment.GetEnvironmentVariable("Authentication:Google:ClientId");
                    var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? Environment.GetEnvironmentVariable("Authentication:Google:ClientSecret");
                    options.ClientId =  clientId ?? throw new ArgumentException("Authentication ClientId should be defined in app settings");
                    options.ClientSecret = clientSecret ?? throw new ArgumentException("Authentication: ClientSecret should be defined in app settings");
                    options.SaveTokens = true;

                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.SaveTokens = true;
                    options.ClaimActions.MapJsonKey("picture", "picture");
                }); 
            builder.Services.AddRazorPages();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
                options.AddPolicy("AdminAreaAccess", policy => policy.RequireRole("Admin"));
           
            });
            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapRazorPages();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<AuthenticationRefreshMiddleware>();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
            ).RequireAuthorization("AdminAreaAccess");

            app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();
            SeedData(app);
            app.Run();
        }
        private static void SeedData(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<HomeEstateDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    SeedAdminUser(userManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
        }
        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@homeestate.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdminUser, "admin");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
        }
    }
}
