using HomeEstate.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Data.Seeds
{
    public class LocationSeeder : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasData(
                new Location
                {
                    Id = 1,
                    City = "Sofia",
                    Address = "123 Vitosha Blvd"
                },
                new Location
                {
                    Id = 2,
                    City = "Plovdiv",
                    Address = "45 Kapana Street"
                },
                new Location
                {
                    Id = 3,
                    City = "Varna",
                    Address = "10 Sea Garden Ave"
                },
                new Location
                {
                    Id = 4,
                    City = "Burgas",
                    Address = "78 Central Square"
                }
            );
        }
    }
        
}
