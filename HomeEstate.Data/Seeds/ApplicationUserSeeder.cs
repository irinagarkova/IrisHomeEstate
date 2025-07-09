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
    public class ApplicationUserSeeder : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasData(
                new ApplicationUser
                {
                    Id = Guid.Parse("4adc1994-f20e-441b-8142-8743e575d5a0").ToString(),
                    Email = "nqkuv@email.com"
                });
        }
    }
}
