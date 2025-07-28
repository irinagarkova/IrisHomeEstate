using HomeEstate.Data.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Data.Models
{
    public class RentalValidationRequest
    {
        public PropertyListingType ListingType { get; set; }
        public decimal? MonthlyRent { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public int? MinimumLeasePeriod { get; set; }
    }
}
