using HomeEstate.HomeEstateCommon;
using HomeEstate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static HomeEstate.HomeEstateCommon.EntityConstants.PropertyConstants;
namespace HomeEstate.HomeEstate.Data.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {

        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder
               .HasKey(p => p.Id);

            builder
                .Property(p => p.Title)
                .IsRequired(true)
                .HasMaxLength(TitleMaxLength);

            builder
                .Property(p => p.Description)
                .IsRequired(true)
                .HasMaxLength(DescriptionMaxLength);

            builder
                .Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder
                .Property(p => p.Area)
                .IsRequired(true);

            builder
                .Property(p => p.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            builder
                .HasOne(p => p.Owner)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder
                .HasMany(p => p.Images)
                .WithOne(pi => pi.Property)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasQueryFilter(p => !p.IsDeleted);
        }

    }
}
