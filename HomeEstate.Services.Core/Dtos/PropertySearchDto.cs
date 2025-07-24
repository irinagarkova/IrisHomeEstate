using HomeEstate.Data.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
    public class PropertySearchDto
    {
        public string? Location { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRent { get; set; }
        public decimal? MaxRent { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public PropertyListingType? ListingType { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public bool? PetsAllowed { get; set; }
        public bool? IsFurnished { get; set; }
        public string? SortBy { get; set; }
    }
}
