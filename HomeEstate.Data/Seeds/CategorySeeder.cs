using HomeEstate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Data.Seeds
{
    public class CategorySeeder : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category
                {
                    Id = 1,
                    Name = "Apartment"
                },
                new Category
                {
                    Id = 2,
                    Name = "House"
                },
                new Category
                {
                    Id = 3,
                    Name = "Office"
                },
                new Category
                {
                    Id = 4,
                    Name = "Villa"
                }
            );
        }
    }
}
