// SportsStore.Domain/Models/Category.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Để dùng List
namespace SportsStore.Domain.Models
{
	[Table("Categories")]
	public class Category
	{
		public int CategoryID { get; set; }
		public string Name { get; set; } = string.Empty;
		public ICollection<Product> Products { get; set; } = new List<Product>();
	}
}