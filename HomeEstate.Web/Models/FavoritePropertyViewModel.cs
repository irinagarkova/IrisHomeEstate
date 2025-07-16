namespace HomeEstate.Web.Models
{
	public class FavoritePropertyViewModel
	{
		public int PropertyId { get; set; }
		public string Title { get; set; } = null!;
		public string Address { get; set; } = null!;
		public decimal Price { get; set; }
		public string ImageUrl { get; set; } = null!;

		public string UserId { get; set; } = null!;
	}
}
