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
    }
}
