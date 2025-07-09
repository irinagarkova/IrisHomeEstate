using HomeEstate.Models;
using System.ComponentModel.DataAnnotations;

namespace HomeEstate.Web.Models
{
    public class AddAndUpdatePropertyViewModel
    {
        public int Id { get; set; }

        [Required()]
        [MinLength(5, ErrorMessage = "Трябва да е повече от 5")]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Area { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public int CategoryId { get; set; }

    }
}
