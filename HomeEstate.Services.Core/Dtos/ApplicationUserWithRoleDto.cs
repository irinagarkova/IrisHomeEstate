using HomeEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
    public class ApplicationUserWithRoleDto : ApplicationUser
    {
        // Role-related properties
        public List<string> Roles { get; set; } = new List<string>();

        // Computed properties for business logic
        public string RolesDisplay => Roles.Any() ? string.Join(", ", Roles) : "No Roles";

        public string HighestRole
        {
            get
            {
                if (Roles.Contains("Admin")) return "Admin";
                if (Roles.Contains("User")) return "User";
                return "None";
            }
        }

        public bool IsAdmin => Roles.Contains("Admin");
        public bool IsUser => Roles.Contains("User");
        public bool HasMultipleRoles => Roles.Count > 1;
    }
}
