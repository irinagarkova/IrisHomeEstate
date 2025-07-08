
using HomeEstate.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Data.Configurations
{
    public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder
                .HasKey(pi => pi.Id);

            builder
                .Property(pi => pi.ImageUrl)
                .IsRequired(true)
                .HasMaxLength(500);

            builder
                .Property<bool>("IsDeleted")
                .HasDefaultValue(false);

            builder
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Restrict); // We'll handle soft delete manually

            builder
                .HasQueryFilter(u => u.IsDeleted == false);
        }
    }
}
