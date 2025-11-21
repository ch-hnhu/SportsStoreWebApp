// SportsStore.Domain/Models/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SportsStore.Domain.Models
{
	[Table("Products")] // Ánh xạ lớp Product tới bảng "Products"
	public class Product
	{
		public int ProductID { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
		public string Name { get; set; } = string.Empty;
		[Required(ErrorMessage = "Vui lòng nhập mô tả")]
		public string Description { get; set; } = string.Empty;
		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Vui lòng nhập giá trị dương")]
		public decimal Price { get; set; }
		[Required(ErrorMessage = "Vui lòng chỉ định một danh mục")]
		public string Category { get; set; } = string.Empty; // Tạm thời dùng string cho CategoryName


		// Thay đổi sang int CategoryId nếu muốn dùng mối quan hệ
		public int CategoryId { get; set; }
		public Category? CategoryRef { get; set; } // Navigation property
		public string? ImageUrl { get; set; } // Thêm trường ImageUrl
	}
}