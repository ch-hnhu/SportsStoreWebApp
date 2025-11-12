using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Đảm bảo đã import namespace này
using System; // Cần cho InvalidOperationException

namespace SportsStoreWebApp.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
	}

	public IActionResult Index()
	{
		ViewBag.HtmlContent = "<p><em>Thông báo quan trọng!</em></p>";
		_logger.LogInformation("Đã yêu cầu trang chủ.");
		return View();
	}
	public IActionResult AboutUs()
	{
		_logger.LogInformation("Yêu cầu trang Giới thiệu.");
		// Có thể truyền một thông điệp đơn giản
		ViewBag.Message = "Đây là trang giới thiệu về chúng tôi.";
		return View(); // Trả về Views/Home/AboutUs.cshtml (cần tạo)
	}
	// Ví dụ trả về JsonResult
	public IActionResult GetServerTime()
	{
		_logger.LogInformation("Đã yêu cầu thời gian máy chủ.");
		return Json(new
		{
			Time = DateTime.Now.ToString("HH:mm:ss"),
			Date = DateTime.Now.ToShortDateString()
		});
	}
	// Ví dụ chuyển hướng
	public IActionResult GoToProductList()
	{
		_logger.LogInformation("Đang chuyển hướng đến danh sách sản phẩm.");
		return RedirectToAction("List", "Product"); // Chuyển hướng đến ProductController.List
	}
	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		// Lấy thông tin lỗi
		var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
		if (exceptionHandlerPathFeature?.Error != null)
		{
			// Sử dụng _logger instance để ghi log lỗi
			_logger.LogError(exceptionHandlerPathFeature.Error, "Một ngoại lệ chưa được xử lý đã xảy ra tại { Path}", exceptionHandlerPathFeature.Path);
		}
		else
		{
			// Ghi log nếu không có thông tin lỗi cụ thể từ exception handler

			_logger.LogWarning("Đã gọi hành động lỗi nhưng không tìm thấy tính năng ngoại lệ cụ thể nào.");

		}
		// Truyền RequestId để người dùng có thể báo cáo lỗi
		ViewBag.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
		return View();
	}
	// Action để giả lập lỗi 500
	public IActionResult SimulateFatalError()
	{
		throw new InvalidOperationException("Đây là một ngoại lệ được cố ý đưa ra để kiểm tra cách xử lý lỗi!!");
	}
}
