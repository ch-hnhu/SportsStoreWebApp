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
		// Ghi log thông tin về yêu cầu truy cập trang sản phẩm
		_logger.LogInformation("Yêu cầu danh sách sản phẩm. Danh mục: {Category}, Trang: {Page}", category, productPage);
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

	// Action Edit (GET): Hiển thị form chỉnh sửa sản phẩm
	public ViewResult Edit(int productId)
	{
		// Tìm sản phẩm theo ID
		Product? product = _repository.Products.FirstOrDefault(p => p.ProductID == productId);
		if (product == null)
		{
			// Nếu không tìm thấy sản phẩm, có thể chuyển hướng hoặc hiển thị lỗi

			_logger.LogWarning("Không tìm thấy sản phẩm có ID { ProductId} để chỉnh sửa.", productId);

			TempData["message"] = $"Sản phẩm có ID {productId} không tồn tại.";

			return View("List", _repository.Products); // Trả về lại danh sách

		}
		return View(product); // Truyền đối tượng Product vào View
	}
	// Action Edit (POST): Xử lý dữ liệu gửi từ form chỉnh sửa
	[HttpPost]
	public IActionResult Edit(Product product) // Model Binding sẽ tựđộng điền dữ liệu vào 'product'
	{
		if (ModelState.IsValid) // Kiểm tra xem Model có hợp lệ không
		{
			_repository.SaveProduct(product); // Lưu sản phẩm (sẽ tạo mới nếu ID = 0, cập nhật nếu ID > 0)
			_logger.LogInformation("Product '{ Sản phẩm '{ProductName}' (ID: {ProductId}) đã được lưu thành công.", product.Name, product.ProductID);
			TempData["message"] = $"{product.Name} đã được lưu thành công!"; // Hiển thị thông báo
			return RedirectToAction("List"); // Chuyển hướng về trang danh sách sản phẩm
		}
		else
		{
			// Dữ liệu không hợp lệ, trả về View để hiển thị lỗi
			_logger.LogWarning("Không xác thực được sản phẩm '{ProductName}'(ID: {ProductId}). Lỗi: {Errors}",
				product.Name ?? "N/A", product.ProductID,
				string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
			return View(product); // Truyền Model trở lại View để giữ dữ liệu đã nhập
		}
	}
	// Action Create: Hiển thị form tạo sản phẩm mới (tương tự Edit nhưng ID = 0)
	public ViewResult Create()
	{
		return View("Edit", new Product()); // Sử dụng lại View Edit với một Product trống
	}
	// Action Delete: Giả lập xóa sản phẩm (chỉ để hoàn thiện logic cơ bản)
	[HttpPost]
	public IActionResult Delete(int productId)
	{
		Product? productToDelete = _repository.Products.FirstOrDefault(p => p.ProductID == productId);
		if (productToDelete != null)
		{
			// FakeProductRepository không có Remove, nên chỉ log
			_logger.LogInformation("Sản phẩm '{ProductName}' (ID: {ProductId}) được đánh dấu để xóa (thực tế không bị xóa trong FakeRepository).", productToDelete.Name, productToDelete.ProductID);
			TempData["message"] = $"{productToDelete.Name} đã được đánh dấu xóa!";
		}
		else
		{
			TempData["message"] = $"Sản phẩm có ID {productId} không tồn tại để xóa.";

		}
		return RedirectToAction("List");
	}
	public IActionResult FilterProducts(ProductFilter filter) // Model Binding cho ProductFilter
	{
		_logger.LogInformation("Lọc sản phẩn theo Category: {Category}, MinPrice: {MinPrice}, MaxPrice: { MaxPrice}, InStockOnly: { InStock}", filter.Category, filter.MinPrice, filter.MaxPrice, filter.InStockOnly);
		// Logic lọc sản phẩm dựa trên filter
		var filteredProducts = _repository.Products;
		if (!string.IsNullOrEmpty(filter.Category))
		{
			filteredProducts = filteredProducts.Where(p => p.Category ==
		   filter.Category);
		}
		if (filter.MinPrice.HasValue)
		{
			filteredProducts = filteredProducts.Where(p => p.Price >=
		filter.MinPrice.Value);
		}
		if (filter.MaxPrice.HasValue)
		{
			filteredProducts = filteredProducts.Where(p => p.Price <=
		   filter.MaxPrice.Value);
		}
		// Nếu InStockOnly = true, thì lọc thêm điều kiện này
		// if (filter.InStockOnly) { filteredProducts = filteredProducts.Where(p => p.IsInStock());
		return View("List", filteredProducts.ToList()); // Tái sử dụng View List
	}
}