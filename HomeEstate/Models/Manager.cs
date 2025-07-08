using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeEstate.Models
{
    [Comment("Manager in the system")]
    public class Manager
    {
        [Comment("Manager identifier")]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        [Comment("Manager's user entity")]
        public string UserId { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;

        public virtual ICollection<Property> ManagedProperties { get; set; }
            = new HashSet<Property>();
    }
}
