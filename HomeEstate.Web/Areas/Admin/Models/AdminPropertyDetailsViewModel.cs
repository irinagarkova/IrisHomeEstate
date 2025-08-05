using HomeEstate.Data.Models.Enum;
using HomeEstate.Web.Models;

namespace HomeEstate.Web.Areas.Admin.Models
{
    public class AdminPropertyDetailsViewModel : DetailsViewModel
    {
        public int Id{ get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
