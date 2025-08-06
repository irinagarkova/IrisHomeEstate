using HomeEstate.Data;
using HomeEstate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HomeEstate.Services.Core.Tests.Infrastructure
{
    public class TestHomeEstateDbContext : IdentityDbContext<ApplicationUser>
    {
        public TestHomeEstateDbContext(DbContextOptions<TestHomeEstateDbContext> options) : base(options)
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
            base.OnModelCreating(builder);

            // Apply only the configurations, NOT the seeds
            builder.ApplyConfigurationsFromAssembly(
                Assembly.GetAssembly(typeof(HomeEstateDbContext)),
                t => !t.Name.Contains("Seeder") // Exclude all seeder classes
            );
        }
    }
}