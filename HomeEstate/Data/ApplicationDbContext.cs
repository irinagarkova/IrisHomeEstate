using HomeEstate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Reflection;

namespace HomeEstate.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
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

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
