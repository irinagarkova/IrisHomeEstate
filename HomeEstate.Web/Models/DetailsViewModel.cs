namespace HomeEstate.Web.Models
{
    using global::HomeEstate.Models;

    public class DetailsViewModel
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Area { get; set; }

        public DateTime CreatedOn { get; set; }

        public ApplicationUser Owner { get; set; } = null!;

        public Location Location { get; set; } = null!;

        public Category Category { get; set; } = null!;


        public virtual ICollection<PropertyImage> Images { get; set; }
            = new List<PropertyImage>();
    }
}
 