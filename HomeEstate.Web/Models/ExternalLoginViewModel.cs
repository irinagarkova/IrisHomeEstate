using System.ComponentModel.DataAnnotations;

namespace HomeEstate.Web.Models
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string ReturnUrl { get; set; }
    }
}
