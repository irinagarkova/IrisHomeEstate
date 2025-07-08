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

        [Comment("Property Address")]
        public string Address { get; set; } = null!;

        [Comment("Property City")]
        public string City { get; set; } = null!;

        [Comment("Property Area")]
        public int Area { get; set; }

        [Comment("Property CreadetOn")]
        public DateTime CreatedOn { get; set; }


        [Comment("Shows if property is deleted")]
        public bool IsDeleted { get; set; }


        [Comment("Property Owner")]
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Навигационно prop за снимки ПИТАЙ СТЕФАН 
        [Comment("Images url")]
        public virtual ICollection<PropertyImage> Images { get; set; } 
            = new HashSet<PropertyImage>();

    }
}
