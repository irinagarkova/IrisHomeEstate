using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
	public class PropertyImageDto
	{
		public int Id { get; set; }
		public int PropertyId { get; set; }

		public string ImageUrl { get; set; } = null!;

	}
}
