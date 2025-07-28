using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
    public class RentalStatisticsDto
    {
        public int TotalRentalProperties { get; set; }
        public decimal AverageMonthlyRent { get; set; }
        public decimal TotalMonthlyIncome { get; set; }
        public int FurnishedProperties { get; set; }
        public int PetFriendlyProperties { get; set; }
        public int PropertiesWithParking { get; set; }
    }
}
