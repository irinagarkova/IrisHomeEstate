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
                 .Property(p => p.Address)
                 .IsRequired(true)
                 .HasMaxLength(AddressMaxLength);

            builder
                .Property(p => p.City)
                .IsRequired(true)
                .HasMaxLength(CityMaxLength);

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
        }
    }
}
            