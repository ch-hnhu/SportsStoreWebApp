using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SportsStoreWebApp.Controllers
{
	public class TestController : Controller
	{
		private readonly ILogger<TestController> _logger;

		public TestController(ILogger<TestController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			ViewBag.Message = "Chào mừng bạn đến với Cửa hàng Thể thao! Đây là trang Test!";
			return View();
		}

		public IActionResult HelloWorld()
		{
			return Content("Xin chào từ Action HelloWorld của TestController!");
		}

		public IActionResult Welcome(string name = "Khách")
		{
			return Content($"Chào mừng {name} đến với trang Test!");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
	}
}