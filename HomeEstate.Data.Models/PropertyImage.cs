using Microsoft.EntityFrameworkCore;

namespace HomeEstate.Models
{
    public class PropertyImage
    {
        [Comment("PropertyImage Id")]
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        [Comment("PropertyImage ImageUrl")]
        public string ImageUrl { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}