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
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .Property(u => u.ProfilePictureURL)
                .IsRequired(false)
                .HasMaxLength(500);

            builder
                .Property<bool>("IsDeleted")
                .HasDefaultValue(false);

           
            builder
                .HasMany(u => u.Properties)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(u => u.FavoriteProperties)
                .WithOne(fp => fp.User)
                .HasForeignKey(fp => fp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадно ако изтриеш потребителя да махне и фаворитите.

            
            builder
                .HasQueryFilter(u => u.IsDeleted == false);
        }
    }
}
