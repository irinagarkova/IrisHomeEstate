
using HomeEstate.Data.Models.Enum;
using HomeEstate.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HomeEstate.Services.Core.Dtos
{
    public class PropertyDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Area { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public string OwnerId { get; set; } = null!;
        public ApplicationUserDto Owner { get; set; } = null!;

        public int LocationId { get; set; }
        public LocationDto Location { get; set; } = null!;

        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; } = null!;

        public int FavoriteCount { get; set; }
        public PropertyListingType ListingType { get; set; } = PropertyListingType.Sale;
        public decimal? MonthlyRent { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public int? MinimumLeasePeriod { get; set; }
        public bool PetsAllowed { get; set; }
        public bool IsFurnished { get; set; }
        public DateTime? AvailableFrom { get; set; }
		public bool IsParking { get; set; }

		public PropertyType PropertyType { get; set; }
		public virtual ICollection<PropertyImageDto> Images { get; set; }
            = new List<PropertyImageDto>();
    }
}
