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
        public ApplicationUser Owner { get; set; } = null!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public virtual ICollection<PropertyImage> Images { get; set; }
            = new List<PropertyImage>();
    }
}
