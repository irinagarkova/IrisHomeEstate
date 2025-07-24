using HomeEstate.Data.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace HomeEstate.Models
{
    public class Property
    {
        [Comment("Property identifier")]
        public int Id { get; set; }

        [Comment("Property Tittle")]
        public string Title { get; set; } = null!;

        [Comment("Property Description")]
        public string Description { get; set; } = null!;

        [Comment("Property Price")]
        public decimal Price { get; set; }

        [Comment("Property Area")]
        public int Area { get; set; }

        [Comment("Property CreadetOn")]
        public DateTime CreatedOn { get; set; }


        [Comment("Shows if property is deleted")]
        public bool IsDeleted { get; set; }


        [Comment("Property Owner")]
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Comment("Property Listing Type - Sale, Rent or Both")]
        public PropertyListingType ListingType { get; set; } = PropertyListingType.Sale;

        [Comment("Monthly rent if property is for rent")]
        public decimal? MonthlyRent { get; set; }

        [Comment("Security deposit for rental properties")]
        public decimal? SecurityDeposit { get; set; }

        [Comment("Minimum lease period in months")]
        public int? MinimumLeasePeriod { get; set; }

        [Comment("Indicates if pets are allowed for rentals")]
        public bool? PetsAllowed { get; set; }

        [Comment("Indicates if property is furnished")]
        public bool? IsFurnished { get; set; }

        public bool? IsParking { get; set; }

        public PropertyType PropertyType { get; set; }

        [Comment("Available from date")]
        public DateTime? AvailableFrom { get; set; }

        [Comment("Images url")]
        public virtual ICollection<PropertyImage> Images { get; set; } 
            = new HashSet<PropertyImage>();

    }
}
