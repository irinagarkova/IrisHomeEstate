using HomeEstate.Data.Models.Enum;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using System.ComponentModel.DataAnnotations;

namespace HomeEstate.Web.Models
{
    public class PropertyViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Area { get; set; }

        public DateTime CreatedOn { get; set; }
		public int FavoriteCount { get; set; }
		public bool IsFavorite { get; set; }
      
        public ApplicationUserDto Owner { get; set; }

        public LocationDto Location { get; set; }

        public CategoryDto Category { get; set; }

        public ICollection<PropertyImageDto> Images { get; set; }

        public PropertyListingType ListingType { get; set; } = PropertyListingType.Sale;
        public PropertyType PropertyType { get; set; } = PropertyType.OneBedroom;
    }
}
