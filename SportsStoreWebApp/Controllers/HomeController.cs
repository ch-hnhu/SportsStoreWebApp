using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Đảm bảo đã import namespace này
using System; // Cần cho InvalidOperationException
using Microsoft.Extensions.Options; // Cần cho IOptions
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Models;
using SportsStoreWebApp.Configurations;

namespace SportsStoreWebApp.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger; // Khai báo một biến để lưu trữ ILogger
	private readonly IProductRepository _repository;
	private readonly PagingSettings _pagingSettings; // Khai báo thuộc tính để lưu cấu hình phân trang

	// Constructor của HomeController
	// ASP.NET Core sẽ tự động inject ILogger vào đây
	public HomeController(IProductRepository repository, ILogger<HomeController> logger, IOptions<PagingSettings> pagingSettings)
	{
		_repository = repository;
		_logger = logger;
		_pagingSettings = pagingSettings.Value; // Lấy đối tượng PagingSettings từ IOptions
	}
	public IActionResult Index(string? category = null, int productPage = 1)
	{
		// Ghi log thông tin về yêu cầu truy cập trang sản phẩm

		_logger.LogInformation("Yêu cầu danh sách sản phẩm. Danh mục: { Category}, Trang: { Page}", category, productPage);
		// Lấy số sản phẩm trên mỗi trang từ cấu hình PagingSettings
		int itemsPerPage = _pagingSettings.ItemsPerPage;
		// int maxPagesToShow = _pagingSettings.MaxPagesToShow; // Có thể dùng sau nếu muốn giới hạn số nút trang hiển thị
		// Lọc sản phẩm theo danh mục (nếu category không null hoặc rỗng)
		// Sau đó sắp xếp và thực hiện phân trang (Skip/Take)
		var productsQuery = _repository.Products
		.Where(p => category == null || p.Category == category);
		var products = productsQuery
		.OrderBy(p => p.ProductID) // Quan trọng: Sắp xếp trước khi Skip / Take để đảm bảo phân trang đúng thứ tự
		.Skip((productPage - 1) * itemsPerPage) // Bỏ qua các sản phẩm của các trang trước đó
		.Take(itemsPerPage) // Lấy số sản phẩm bằng ItemsPerPage cho trang hiện tại
		.ToList(); // Chuyển kết quả sang List để truyền cho View

		// Chuẩn bị dữ liệu cần thiết cho View thông qua ViewBag
		ViewBag.Categories = _repository.Products
		.Select(p => p.Category)
		.Distinct()
		.OrderBy(c => c)
		.ToList();
		ViewBag.CurrentCategory = category ?? "Tất cả sản phẩm"; // Danh mục hiện tại

		ViewBag.CurrentPage = productPage; // Trang hiện tại
		ViewBag.TotalItems = productsQuery.Count(); // Tổng số sản phẩm SAU KHI lọc, nhưng TRƯỚC KHI phân trang

		ViewBag.ItemsPerPage = itemsPerPage; // Số sản phẩm trên mỗi trang

		// Ghi log thông tin về số lượng sản phẩm được trả về
		//_logger.LogInformation("Trả về {ProductCount} sản phẩm cho trang { Page}. Tổng số sản phẩm: { TotalItems}", products.Count, productPage, ViewBag.TotalItems);
		// Trả về View với danh sách sản phẩm của trang hiện tại làm Model
		return View(products);
	}
}
