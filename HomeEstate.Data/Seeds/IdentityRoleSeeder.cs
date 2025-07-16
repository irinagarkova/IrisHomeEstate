using HomeEstate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Data.Seeds
{
	public class IdentityRoleSeeder : IEntityTypeConfiguration<IdentityRole>
	{
		public void Configure(EntityTypeBuilder<IdentityRole> builder)
		{
			builder.HasData(
				new IdentityRole
				{
					Id = "1",
					Name = "User",
					NormalizedName = "USER"
				},
				new IdentityRole
				{
					Id = "2",
					Name = "Admin",
					NormalizedName = "ADMIN"
				});
		}
	}
}
