using Microsoft.EntityFrameworkCore;

namespace HomeEstate.Models
{
    public class Location
    {
        [Comment("Location ID")]
        public int Id { get; set; }

        [Comment("Location CityName")]
        public string City { get; set; } = null!;
        public string Address{ get; set; } = null!;
        [Comment("Shows if location is deleted")]
        public bool IsDeleted { get; set; } = false;
    }
}
