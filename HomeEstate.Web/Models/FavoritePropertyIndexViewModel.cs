using HomeEstate.Services.Core.Dtos;

namespace HomeEstate.Web.Models
{
    public class FavoritePropertyIndexViewModel
    {
        public Pagination<PropertyViewModel> Properties { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = "newest";
        public string Category { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public List<int> PageSizes { get; set; } = new() { 6, 12, 24, 36 };
    }
}