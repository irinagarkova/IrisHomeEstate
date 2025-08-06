using HomeEstate.Data.Models.Enum;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
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
		public IEnumerable<SelectListItem> Locations { get; set; } = new List<SelectListItem>();

        [Required]
		public List<IFormFile> Images { get; set; } = new();
        public List<PropertyImageDto> ExistingImages { get; set; } = new(); 
        public PropertyListingType ListingType { get; set; } = PropertyListingType.Sale; 
        public decimal? MonthlyRent { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public int? MinimumLeasePeriod { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public bool PetsAllowed { get; set; }
        public bool IsFurnished { get; set; }
        public bool IsParking { get; set; }
        public bool IsActive { get; set; } 
      
        public PropertyType PropertyType { get; set; }
    }
}
