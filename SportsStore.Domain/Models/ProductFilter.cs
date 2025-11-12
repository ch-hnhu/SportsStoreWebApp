// SportsStore.Domain/Models/ProductFilter.cs (hoặc có thể là class nội bộ trong Controller)
namespace SportsStore.Domain.Models
{
	public class ProductFilter
	{
		public string? Category { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public bool InStockOnly { get; set; }
	}
}