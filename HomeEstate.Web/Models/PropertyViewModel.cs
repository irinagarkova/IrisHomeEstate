using HomeEstate.Models;
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
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;

        public Location Location { get; set; } = null!;

        public Category Category { get; set; } = null!;

    
        public virtual ICollection<PropertyImage> Images { get; set; }
            = new List<PropertyImage>();
    }
}
