using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SportsStoreWebApp.Controllers
{
	public class AdminController : Controller
	{
		private IProductRepository _repository;
		public AdminController(IProductRepository repo)
		{
			_repository = repo;
		}
		public ViewResult Index() => View(_repository.Products); // Hiển thị tất cả sản phẩm trong Admin Panel

		// POST: Admin/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Product product)
		{
			if (ModelState.IsValid)
			{
				await _repository.SaveProduct(product);
				TempData["message"] = $"{product.Name} đã được lưu thành công.";

				return RedirectToAction(nameof(Index)); // Chuyển hướng về trang Index của Admin

			}
			else
			{
				// Dữ liệu không hợp lệ, hiển thị lại form
				return View(product);
			}
		}
		// GET: Admin/Create
		public ViewResult Create() => View("Edit", new Product()); // Tái sử dụng View Edit cho Create
																   // POST: Admin/Delete
		[HttpPost]

		public async Task<IActionResult> Delete(int productId)
		{
			Product? deletedProduct = await
		   _repository.DeleteProduct(productId);
			if (deletedProduct != null)
			{
				TempData["message"] = $"{deletedProduct.Name} đã được xóa.";

			}
			else
			{
				TempData["message"] = $"Không tìm thấy sản phẩm có ID: {productId} để xóa.";

				TempData["messageType"] = "danger"; // Báo lỗi
			}
			return RedirectToAction(nameof(Index));
		}
		// Controllers/AdminController.cs
		public async Task<IActionResult> Edit(int productId)
		{
			Product? product;
			if (productId == 0)
			{
				product = new Product();
			}
			else
			{
				// Sử dụng Include để tải Category liên quan
				product = await _repository.Products
					.Include(p => p.CategoryRef) // Bỏ comment nếu có CategoryRef và muốn tải
					.FirstOrDefaultAsync(p => p.ProductID == productId);
				if (product == null)
				{
					return NotFound();
				}
			}
			// Nếu bạn có danh mục riêng, bạn có thể tải chúng vào ViewBag để dùng cho dropdown
			// ViewBag.Categories = await _context.Categories.ToListAsync();
			return View(product);
		}
	}
}