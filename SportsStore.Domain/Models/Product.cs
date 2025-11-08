using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Models
{
	public class Product
	{
		[Key]
		public int ProductID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public string Category { get; set; } = string.Empty;
	}
}