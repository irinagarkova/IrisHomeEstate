using HomeEstate.Data.Seeds;
using HomeEstate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace HomeEstate.Data
{
    public class HomeEstateDbContext : IdentityDbContext<ApplicationUser>
    {

        public HomeEstateDbContext(DbContextOptions<HomeEstateDbContext> options) : base(options)
        { 
        }

        public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<PropertyImage> PropertyImages { get; set; } = null!;
        public virtual DbSet<FavoriteProperty> FavoriteProperties { get; set; } = null!;
        public virtual DbSet<ApplicationUser> ApplicationUser { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CategorySeeder());
            builder.ApplyConfiguration(new PropertySeeder());
            builder.ApplyConfiguration(new LocationSeeder());
            builder.ApplyConfiguration(new PropertyImageSeeder());
            builder.ApplyConfiguration(new ApplicationUserSeeder());

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);

        }
    }
}
