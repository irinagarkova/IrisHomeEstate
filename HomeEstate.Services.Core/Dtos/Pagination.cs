using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Dtos
{
    public class Pagination<T> : IPagination
    {
        public List<T> Items { get; set; } = new();
    }
}
