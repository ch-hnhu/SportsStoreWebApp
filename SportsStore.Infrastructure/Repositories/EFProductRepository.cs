// SportsStore.Infrastructure/Repositories/EFProductRepository.cs
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using Microsoft.EntityFrameworkCore; // Để dùng ToListAsync, etc.
namespace SportsStore.Infrastructure.Repositories
{
	public class EFProductRepository : IProductRepository
	{
		private ApplicationDbContext _context;
		public EFProductRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public IQueryable<Product> Products => _context.Products; // Chỉ đơn giản trả về DbSet

		// Ví dụ sử dụng .Update() khi đối tượng KHÔNG được theo dõi bởi DbContext hiện tại
		public async Task UpdateProductDetached(Product product)
		{
			_context.Products.Update(product); // Đánh dấu đối tượng là Modified
			await _context.SaveChangesAsync();
		}
		// Triển khai phương thức SaveProduct từ IProductRepository
		public async Task SaveProduct(Product product)
		{
			if (product.ProductID == 0) // Sản phẩm mới (đã học ở tuần 9)
			{
				_context.Products.Add(product);
			}
			else // Sản phẩm đã tồn tại (Update)
			{
				// Tìm sản phẩm trong DB bằng ID
				Product? dbEntry = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == product.ProductID);
				if (dbEntry != null)
				{
					// Cập nhật các thuộc tính của đối tượng được theo dõi
					dbEntry.Name = product.Name;
					dbEntry.Description = product.Description;
					dbEntry.Price = product.Price;
					dbEntry.Category = product.Category;
					dbEntry.ImageUrl = product.ImageUrl;
					// EF Core tự động theo dõi các thay đổi này
				}
			}
			await _context.SaveChangesAsync(); // Lưu tất cả thay đổi vào DB (INSERT hoặc UPDATE)
		}
		// Triển khai phương thức DeleteProduct (sẽ học sau)
		public async Task<Product?> DeleteProduct(int productId)
		{
			Product? dbEntry = await
		   _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
			if (dbEntry != null)
			{
				_context.Products.Remove(dbEntry);
				await _context.SaveChangesAsync();
			}
			return dbEntry;
		}
	}
}