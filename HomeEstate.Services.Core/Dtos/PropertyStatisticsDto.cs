using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{ 
    public class PropertyStatisticsDto
    {
        public int TotalProperties { get; set; }
        public int PropertiesForSale { get; set; }
        public int PropertiesForRent { get; set; }
        public int ActiveListings { get; set; }
        public int TotalViews { get; set; }
        public int TotalFavorites { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AverageRent { get; set; }
    }
}

