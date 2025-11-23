using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SportsStore.Infrastructure;
using NuGet.Protocol.Core.Types;

namespace SportsStoreWebApp.Controllers
{
	[Authorize(Roles = "Admin")] // Yêu cầu đăng nhập để truy cập tất cả các action trong AdminController
	public class AdminController : Controller
	{
		private readonly ILogger<AdminController> _logger;
		private IProductRepository _repository;
		private ApplicationDbContext _context;

		public AdminController(ILogger<AdminController> logger, IProductRepository repo, ApplicationDbContext context)
		{
			_logger = logger;
			_repository = repo;
			_context = context;
		}

		public IActionResult Index()
		{
			var products = _repository.Products
				.Include(p => p.CategoryRef); // Tải Category liên quan
			return View(products);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
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
				ViewBag.Categories = await _context.Categories.ToListAsync();
				return View(product);
			}
		}
		// GET: Admin/Create
		public async Task<IActionResult> Create()
		{
			ViewBag.Categories = await _context.Categories.ToListAsync();
			return View("Edit", new Product()); // Tái sử dụng View Edit cho Create
		}

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
			// Tải danh sách Categories để hiển thị trong dropdown
			ViewBag.Categories = await _context.Categories.ToListAsync();
			return View(product);
		}

	}
}