using HomeEstate.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static HomeEstate.HomeEstateCommon.EntityConstants.LocationConstants;

namespace HomeEstate.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder
                .HasKey(l => l.Id);

            builder
                .Property(l => l.City)
                .IsRequired(true)
                .HasMaxLength(CityNameMaxLength);

            builder
                .Property(l => l.Address)
                .IsRequired(true)
                .HasMaxLength(200);

            builder
                .Property(l => l.IsDeleted)
                .HasDefaultValue(false);

            // Add query filter for soft delete
            builder
                .HasQueryFilter(l => !l.IsDeleted);
        }
    }
}