using HomeEstate.Data.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
    public class SearchPropertyDto
    {
        public string Location { get; set; }
        public int CategoryId { get; set; }
        public decimal MaxPrice { get; set; }
        public string SortBy { get; set; }
        public decimal? MinPrice { get; set; }  // Добавете това
        public decimal? MaxRent { get; set; }
        public decimal? MinRent { get; set; }   // Добавете това
        public int? MaxArea { get; set; }
        public int? MinArea { get; set; }       // Добавете това
        public PropertyListingType? ListingType { get; set; }
        public int? Bedrooms { get; set; }
        public bool? PetsAllowed { get; set; }
        public bool? IsFurnished { get; set; }
    }
}
