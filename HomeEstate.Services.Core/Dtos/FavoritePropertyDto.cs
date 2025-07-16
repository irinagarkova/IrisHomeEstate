using HomeEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
	public class FavoritePropertyDto
	{
		public int PropertyId { get; set; } // FK към Property
		public PropertyDto Property { get; set; } = null!;

		public string UserId { get; set; } = null!; // FK ApplicationUser 
		public ApplicationUserDto User { get; set; } = null!;
	}
}
