// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using System.Linq;
using System.Threading.Tasks; // Để dùng async/await
using SportsStore.Domain.Models; // Để dùng Product
using Microsoft.EntityFrameworkCore; // Để dùng ToListAsync, CountAsync, FirstOrDefaultAsync
using SportsStore.Infrastructure; // Để dùng ApplicationDbContext
namespace SportsStore.WebUI.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;
		private IProductRepository _repository;
		public int PageSize = 4; // Kích thước trang
		public ProductController(IProductRepository repo, ApplicationDbContext context)
		{
			_repository = repo;
			_context = context;
		}
		public async Task<IActionResult> List(string category, int productPage = 1)
		{
			int pageSize = 4; // Ví dụ
			var productsQuery = _context.Products.AsQueryable(); // Bắt đầu truy vấn
			if (!string.IsNullOrEmpty(category))
			{
				productsQuery = productsQuery.Where(p => p.Category == category);
			}
			var products = await productsQuery
				.OrderBy(p => p.ProductID) // Sắp xếp để phân trang nhất quán
				.Skip((productPage - 1) * pageSize)
 				.Take(pageSize)
 				.ToListAsync(); // Thực thi truy vấn bất đồng bộ

			// Tính tổng số sản phẩm cho phân trang (cần async)
			var totalItems = await productsQuery.CountAsync();
			ViewBag.TotalItems = totalItems;
			ViewBag.ItemsPerPage = pageSize;
			ViewBag.CurrentPage = productPage;
			ViewBag.CurrentCategory = category;
			return View(products);
		}
		// Action Edit (để thử Create, sẽ hoàn thiện Update sau)
		public async Task<IActionResult> Edit(int productId = 0)
		{
			Product? product = productId == 0 ? new Product() : await _repository.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
			if (product == null && productId != 0)
			{
				return NotFound();
			}
			return View(product);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Product product)
		{
			if (ModelState.IsValid)
			{
				await _repository.SaveProduct(product); // Gọi SaveProduct để thêm(hoặc cập nhật)

				TempData["message"] = $"{product.Name} đã được lưu thành công.";

				return RedirectToAction("List");
			}
			else
			{
				return View(product);
			}
		}
		// Ví dụ trong một Service hoặc Controller
		public async Task AddNewProduct(Product newProduct)
		{
			_context.Products.Add(newProduct); // Đánh dấu đối tượng để thêm
			await _context.SaveChangesAsync(); // Lưu thay đổi vào DB (thực hiện INSERT)
		}
		public async Task AddMultipleProducts(IEnumerable<Product> products)
		{
			_context.Products.AddRange(products); // Thêm nhiều đối tượng cùng lúc
			await _context.SaveChangesAsync();
		}
	}
}

