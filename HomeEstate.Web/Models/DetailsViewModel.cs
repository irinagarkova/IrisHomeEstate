using HomeEstate.Data.Models.Enum;

namespace HomeEstate.Web.Models
{ 

    public class DetailsViewModel
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Area { get; set; }

        public DateTime CreatedOn { get; set; }

        public string OwnerFullName { get; set; } = null!;

        public string LocationName { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public List<string> Images { get; set; } = new();

		public PropertyListingType ListingType { get; internal set; }
		//public virtual ICollection<PropertyImage> Images { get; set; }
		//     = new List<PropertyImage>();
	}
}
 