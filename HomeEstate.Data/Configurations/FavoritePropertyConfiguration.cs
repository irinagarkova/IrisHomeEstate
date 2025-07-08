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
    public class FavoritePropertyConfiguration : IEntityTypeConfiguration<FavoriteProperty>
    {
        public void Configure(EntityTypeBuilder<FavoriteProperty> builder)
        {
            builder
                .HasKey(fp => new { fp.PropertyId, fp.UserId });

            builder
                .HasOne(fp => fp.Property)
                .WithMany()
                .HasForeignKey(fp => fp.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(fp => fp.User)
                .WithMany(u => u.FavoriteProperties)
                .HasForeignKey(fp => fp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
