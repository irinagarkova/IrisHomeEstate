using HomeEstate.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static HomeEstate.HomeEstateCommon.EntityConstants.LocationConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Property<bool>("IsDeleted")
                .HasDefaultValue(false);
        }
    }
}
