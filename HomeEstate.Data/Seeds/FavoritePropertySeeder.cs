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
    public class FavoritePropertySeeder : IEntityTypeConfiguration<FavoriteProperty>
    {
        public void Configure(EntityTypeBuilder<FavoriteProperty> builder)
        {
            builder.HasData(
                new FavoriteProperty
                {
                    UserId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                    PropertyId = 1
                }, new FavoriteProperty
                {
                    UserId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                    PropertyId = 2
                }, new FavoriteProperty
                {
                    UserId = "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                    PropertyId = 3
                });
		}
    }
}
