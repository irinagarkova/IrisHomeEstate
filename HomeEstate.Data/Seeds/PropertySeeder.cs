using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeEstate.Models;

namespace HomeEstate.Data.Seeds
{
    public class PropertySeeder : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasData(
                new Property
                {
                    Id = 1,
                    Title = "Luxury Apartment in Sofia",
                    Description = "Modern apartment located in the city center with great amenities.",
                    Price = 250000m,
                    Area = 120,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    OwnerId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                    CategoryId = 1,
                    LocationId = 1
                },
                new Property
                {
                    Id = 2,
                    Title = "Cozy House in the Countryside",
                    Description = "Beautiful countryside house with a big garden.",
                    Price = 180000m,
                    Area = 200,
                    CreatedOn = new DateTime(2025, 7, 3),
                    IsDeleted = false,
                    OwnerId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                    CategoryId = 2,
                    LocationId = 2
                },
                new Property
                {
                    Id = 3,
                    Title = "Commercial Office Space",
                    Description = "Prime commercial location with modern infrastructure and ample parking.",
                    Price = 16000m,
                    Area = 180,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    OwnerId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                    CategoryId = 3,
                    LocationId = 3
                },
                 new Property
                 {
                     Id = 4,
                     Title = "Luxury Apartment with Pool",
                     Description = "High-end condo with premium amenities, pool access, and concierge services.",
                     Price = 27500000m,
                     Area = 800,
                     CreatedOn = DateTime.Now.AddMonths(-9),
                     IsDeleted = false,
                     OwnerId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                     CategoryId = 1,
                     LocationId = 1
                 },
                 new Property
                 {
                     Id = 5,
                     Title = "Spacious Family House",
                     Description = "Perfect family home with large backyard, garage, and excellent school district.",
                     Price = 450000.00m,
                     Area = 2500,
                     CreatedOn = DateTime.Now.AddMonths(-3),
                     IsDeleted = false,
                     OwnerId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                     CategoryId = 4,
                     LocationId = 4
                 }
            );
        }
    }
}
