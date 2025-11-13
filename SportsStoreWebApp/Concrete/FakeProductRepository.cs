// SportsStoreWebApp/Concrete/FakeProductRepository.cs
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using System.Linq;
using System.Collections.Generic;
namespace SportsStoreWebApp.Concrete
{
	public class FakeProductRepository : IProductRepository
	{
		public List<Product> products => new List<Product>
		{
			new Product { ProductID = 1, Name = "Bóng đá Adidas", Description = "Bóng đá dùng cho mọi loại sân, độ bền cao.", Price = 25.00m, Category = "Bóng đá" },
			new Product { ProductID = 2, Name = "Áo đấu đội tuyển A", Description = "Áo đấu chất liệu thoáng khí, thấm hút mồ hôi.", Price = 49.99m, Category = "Quần áo" },
			new Product { ProductID = 3, Name = "Vợt cầu lông Yonex", Description = "Vợt nhẹ, cân bằng tốt, phù hợp cho người chơi trung cấp.", Price = 120.00m, Category = "Cầu lông" },
			new Product { ProductID = 4, Name = "Giày chạy bộ Nike Air", Description = "Giày êm ái, hỗ trợ tối đa cho các buổi tập dài.", Price = 95.00m, Category = "Giày" },
			new Product { ProductID = 5, Name = "Bóng chuyền Mikasa  MVA200", Description = "Bóng chuyền tiêu chuẩn thi đấu quốc tế.", Price =  30.00m, Category = "Bóng chuyền" },
			new Product { ProductID = 6, Name = "Bóng đá Nike Strike", Description = "Bóng đá tập luyện chất lượng cao.", Price = 20.00m, Category = "Bóng đá" },
			new Product { ProductID = 7, Name = "Quần short thể thao", Description = "Quần short co giãn, thoải mái khi vận động.", Price = 25.00m, Category = "Quần áo" }
		};
		public IQueryable<Product> Products => products.AsQueryable();

		public Task SaveProduct(Product product) // Thay đổi return type thành Task
		{
			if (product.ProductID == 0) // Nếu là sản phẩm mới (ID=0), gán ID mới
			{
				product.ProductID = products.Max(p => p.ProductID) + 1;
				products.Add(product);
			}
			else // Nếu là sản phẩm đã có, cập nhật
			{
				Product? existingProduct = products.FirstOrDefault(p =>
			   p.ProductID == product.ProductID);
				if (existingProduct != null)
				{
					existingProduct.Name = product.Name;
					existingProduct.Description = product.Description;
					existingProduct.Price = product.Price;
					existingProduct.Category = product.Category;
				}
			}
			return Task.CompletedTask; // Trả về Task đã hoàn thành
		}

		public Task<Product?> DeleteProduct(int productId)
		{
			Product? product = products.FirstOrDefault(p => p.ProductID == productId);
			if (product != null)
			{
				products.Remove(product);
			}
			return Task.FromResult(product); // Trả về Task với kết quả
		}
	}
}