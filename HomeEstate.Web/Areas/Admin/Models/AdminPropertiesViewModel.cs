using HomeEstate.Services.Core.Dtos;

namespace HomeEstate.Web.Areas.Models
{
    public class AdminPropertiesViewModel
    {
        public PaginatedDto<PropertyDto> Properties { get; set; } = new();
        public string SearchTerm { get; set; } = "";
        public string SortBy { get; set; } = "newest";
        public int[] PageSizes { get; set; } = { 5, 10, 25, 50 };
    }
}
