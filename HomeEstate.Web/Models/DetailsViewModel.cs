using HomeEstate.Data.Models.Enum;
using HomeEstate.Services.Core.Dtos;

namespace HomeEstate.Web.Models
{ 

    public class DetailsViewModel
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Area { get; set; }

        public DateTime CreatedOn { get; set; }

        public ApplicationUserDto Owner { get; set; } = null!;

        public LocationDto Location { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public List<PropertyImageDto> Images { get; set; } = new();

		public PropertyListingType ListingType { get; internal set; }
        public decimal? MonthlyRent { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public int? MinimumLeasePeriod { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public bool PetsAllowed { get; set; }
        public bool IsFurnished { get; set; }
        public bool IsParking { get; set; }
        public bool IsActive { get; set; }
        public int FavoriteCount { get; set; }
        //public virtual ICollection<PropertyImage> Images { get; set; }
        //     = new List<PropertyImage>();
    }
}
 