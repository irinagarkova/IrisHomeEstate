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
					new Location { Id = 1, City = "Sofia", Address = "123 Vitosha Blvd" },
					new Location { Id = 2, City = "Plovdiv", Address = "45 Kapana Street" },
					new Location { Id = 3, City = "Varna", Address = "10 Sea Garden Ave" },
					new Location { Id = 4, City = "Burgas", Address = "78 Central Square" },
					new Location { Id = 5, City = "Vidin", Address = "" },
					new Location { Id = 6, City = "Vratsa", Address = "" },
					new Location { Id = 7, City = "Gabrovo", Address = "" },
					new Location { Id = 8, City = "Kardzhali", Address = "" },
					new Location { Id = 9, City = "Kyustendil", Address = "" },
					new Location { Id = 10, City = "Lovech", Address = "" },
					new Location { Id = 11, City = "Montana", Address = "" },
					new Location { Id = 12, City = "Pazardzhik", Address = "" },
					new Location { Id = 13, City = "Pernik", Address = "" },
					new Location { Id = 14, City = "Pleven", Address = "" },
					new Location { Id = 15, City = "Veliko Tarnovo", Address = "" },
					new Location { Id = 16, City = "Razgrad", Address = "" },
					new Location { Id = 17, City = "Ruse", Address = "" },
					new Location { Id = 18, City = "Silistra", Address = "" },
					new Location { Id = 19, City = "Sliven", Address = "" },
					new Location { Id = 20, City = "Smolyan", Address = "" },
					new Location { Id = 21, City = "Blagoevgrad", Address = "" },
					new Location { Id = 22, City = "Stara Zagora", Address = "" },
					new Location { Id = 23, City = "Targovishte", Address = "" },
					new Location { Id = 24, City = "Haskovo", Address = "" },
					new Location { Id = 25, City = "Shumen", Address = "" },
					new Location { Id = 26, City = "Yambol", Address = "" },
					new Location { Id = 27, City = "Dobrich", Address = "" }
			);
		}
		
	}
}

