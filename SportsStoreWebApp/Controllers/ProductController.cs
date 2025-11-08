// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using System.Linq;
using SportsStoreWebApp.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

public class ProductController : Controller
{
	private readonly IProductRepository _repository;
	private readonly ILogger<ProductController> _logger;
	private readonly PagingSettings _pagingSettings;
	public ProductController(IProductRepository repository, ILogger<ProductController> logger, IOptions<PagingSettings> pagingSettings)
	{
		_repository = repository;
		_logger = logger;
		_pagingSettings = pagingSettings.Value;
		_logger.LogInformation("ProductController đã được tạo.");
	}
	public IActionResult List(string? category = null, int productPage = 1)
	{
		_logger.LogInformation("Yêu cầu danh sách sản phẩm. Danh mục: {Category}, Trang: {Page}", category ?? "Tất cả", productPage);

		int itemsPerPage = _pagingSettings.ItemsPerPage;

		var productsQuery = _repository.Products
			.Where(p => category == null || p.Category == category);

		var products = productsQuery
			.OrderBy(p => p.ProductID)
			.Skip((productPage - 1) * itemsPerPage)
			.Take(itemsPerPage)
			.ToList();

		ViewBag.CurrentCategory = category ?? "Tất cả sản phẩm";
		ViewBag.CurrentPage = productPage;
		ViewBag.TotalItems = productsQuery.Count();
		ViewBag.ItemsPerPage = itemsPerPage;

		_logger.LogInformation("Trả về {ProductCount} sản phẩm cho trang {Page}. Tổng số sản phẩm: {TotalItems}", products.Count, productPage, (int)ViewBag.TotalItems);

		return View(products);
	}

	public IActionResult Details(int id)
	{
		var product = _repository.Products.FirstOrDefault(p => p.ProductID == id);
		if (product == null)
		{
			_logger.LogWarning("Sản phẩm với ID {ProductID} không tìm thấy.", id);
			return NotFound();
		}

		_logger.LogInformation("Hiển thị chi tiết cho sản phẩm ID: {ProductID}", id);
		return View(product);
	}

	// Tạo một Action để kiểm tra ghi log lỗi
	public IActionResult SimulateError()
	{
		try
		{
			_logger.LogWarning("Mô phỏng lỗi để kiểm tra nhật ký...");
			throw new InvalidOperationException("Đây là lỗi kiểm tra từ SimulateError action!");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Đã xảy ra lỗi không mong muốn trong quá trình mô phỏng lỗi!");
		}
		return Content("Kiểm tra đầu ra console/debug của bạn để tìm nhật ký!");
	}
}