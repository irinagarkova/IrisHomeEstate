using HomeEstate.Data;
using HomeEstate.Services.Core.Interfaces;
using HomeEstate.Services.Core.Mappings;
using HomeEstate.Services.Core.Services;
using HomeEstate.Web.Mappings;
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
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


			builder.Services.AddAutoMapper(cfg =>
			{
				cfg.AddProfile<PropertyMappingProfile>();
				cfg.AddProfile<PropertyWebMappingProfile>();
			});

			builder.Services.AddScoped<IPropertyService, PropertyService>();
			builder.Services.AddScoped<IFavoritePropertyService, FavoritePropertyService>();

			builder.Services
			  .AddIdentity<IdentityUser, IdentityRole>(options =>
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
			  .AddDefaultTokenProviders(); // da se dobavi ako  ima vreme
			builder.Services.AddControllersWithViews();
			builder.Services.AddScoped<IPropertyService, PropertyService>();
			builder.Services.AddRazorPages();
			WebApplication app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{

			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}


			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.MapRazorPages();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			app.Run();
		}
	}
}
