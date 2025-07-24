namespace HomeEstate.Web.Models
{
    public class SearchViewModel
    {
        public string Location { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; }



    }
}
