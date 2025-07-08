using Microsoft.AspNetCore.Identity;

namespace HomeEstate.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfilePictureURL { get; set; } 

        // Всеки потребител може да има много имоти
        public virtual ICollection<Property> Properties { get; set; } = new HashSet<Property>();

        // Всеки потребител може да има много любими имоти
        public virtual ICollection<FavoriteProperty> FavoriteProperties { get; set; }
            = new HashSet<FavoriteProperty>();
    }
}
