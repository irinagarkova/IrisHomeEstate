using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
    public class LocationDto
    {
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
