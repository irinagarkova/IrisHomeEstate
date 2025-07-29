using System.ComponentModel.DataAnnotations;

namespace HomeEstate.Web.Models
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        //[Display(Name = "First Name")]
        //public string FirstName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
