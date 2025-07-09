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
    public class PropertyImageSeeder : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder.HasData(
                //Апартамент 
                new PropertyImage
                {
                    Id = 1,
                    PropertyId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&h=600&fit=crop",
                    IsDeleted = false
                },
                new PropertyImage
                {
                    Id = 2,
                    PropertyId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=800&h=600&fit=crop",
                    IsDeleted = false
                },
                new PropertyImage
                {
                    Id = 3,
                    PropertyId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800&h=600&fit=crop",
                    IsDeleted = false
                },

                 // Къща ОК
                 new PropertyImage
                 {
                     Id = 4,
                     PropertyId = 2,
                     ImageUrl = "https://images.unsplash.com/photo-1518780664697-55e3ad937233?w=800&h=600&fit=crop",
                     IsDeleted = false
                 },
                new PropertyImage
                {
                    Id = 5,
                    PropertyId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800&h=600&fit=crop",
                    IsDeleted = false
                },
                new PropertyImage
                {
                    Id = 6,
                    PropertyId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1505693314120-0d443867891c?w=800&h=600&fit=crop",
                    IsDeleted = false
                },
                new PropertyImage
                {
                    Id = 7,
                    PropertyId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800&h=600&fit=crop",
                    IsDeleted = false
                },

                // Офис
                new PropertyImage
                {
                    Id = 8,
                    PropertyId = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&h=600&fit=crop",
                    IsDeleted = false
                },
                new PropertyImage
                {
                    Id = 9,
                    PropertyId = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=800&h=600&fit=crop",
                    IsDeleted = false
                },
                // Вила
                new PropertyImage
                {
                    Id = 10,
                    PropertyId = 4,
                    ImageUrl = "https://img.vila.bg/g/6556/170637.jpg",
                    IsDeleted = false
                },
                new PropertyImage
                {
                    Id = 11,
                    PropertyId = 4,
                    ImageUrl = "https://api.photon.aremedia.net.au/wp-content/uploads/sites/2/umb-media/25922/resort-style-1980s-home-renovation-living-room-vaulted-a-frame-ceiling.jpg?crop=0px%2C1001px%2C1467px%2C825px&resize=720%2C405",
                    IsDeleted = false
                }
            );
        }
    }
}
