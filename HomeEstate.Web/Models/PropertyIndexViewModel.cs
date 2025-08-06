using HomeEstate.Services.Core.Dtos;

namespace HomeEstate.Web.Models
{
    public class PropertyIndexViewModel
    {
        public Pagination<PropertyViewModel> Properties { get; set; } = new();
        public PropertySearchDto SearchCriteria { get; set; } = new();
        public List<int> PageSizes { get; set; } = new() { 6, 12, 24, 36, 48 };
        public string SortBy { get; set; } = "newest";
        public string SearchTerm { get; set; } = string.Empty;
    }
}