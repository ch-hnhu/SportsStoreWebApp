using SportsStore.Domain.Models;
using SportsStoreWebApp.Middleware;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductRepository, FakeProductRepository>();

var app = builder.Build();

app.UseMiddleware<RequestLoggerMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// *** Route cụ thể hơn: Ví dụ cho các URL có cấu trúc rõ ràng cho sản phẩm theo danh mục ***
app.MapControllerRoute(
 	name: "product_by_category",
 	pattern: "san-pham/danh-muc/{category?}", // URL sẽ là /san-pham/danh-muc/bong-da
	defaults: new { controller = "Product", action = "List" }
);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("--- Thực hành C# căn bản ---");

// Tạo ds sp mẫu
List<Product> sampleProducts = new List<Product>
{
	new Product { ProductID = 1, Name = "Bóng đá WorldCup", Description = "Bóng đá chính hãng", Price = 29.99M, Category = "Bóng đá" },
	new Product { ProductID = 1, Name = "Áo đấu CLB A", Description = "Áo đấu cho người hâm mộ", Price = 75.50M, Category = "Quần áo" },
	new Product { ProductID = 1, Name = "Vợt Tennis Pro", Description = "Vợt chuyên nghiệp", Price = 150.00M, Category = "Tennis" },
	new Product { ProductID = 1, Name = "Giày chạy bộ ABC", Description = "Giày thể thao nhẹ", Price = 99.99M, Category = "Giày" },
	new Product { ProductID = 1, Name = "Bóng rổ NBA", Description = "Bóng rổ tiêu chuẩn", Price = 45.00M, Category = "Bóng rổ" },
};

Console.WriteLine("\n--- LINQ: Lọc sản phẩm có giá trên 70 ---");
var expensiveProducts = sampleProducts.Where(p => p.Price > 70.00m);
foreach (var p in expensiveProducts)
{
	Console.WriteLine($"- {p.Name}: ({p.Price})");
}

Console.WriteLine("\n--- LINQ: Lấy sản phẩm đầu tiên thuộc danh mục 'Bóng đá' ---");
var firstFootballProduct = sampleProducts.FirstOrDefault(p => p.Category == "Bóng đá");
if (firstFootballProduct != null)
{
	Console.WriteLine($"- {firstFootballProduct.Name}");
}
else
{
	Console.WriteLine("Không tìm thấy sản phẩm bóng đá.");
}

Console.WriteLine("\n--- Async/Await: Mô phỏng thao tác bất đồng bộ ---");
async Task SimulateDataFetchAsync()
{
	Console.WriteLine("Đang bắt đầu lấy dữ liệu (mất 2 giây)...");
	await Task.Delay(2000); // Mô phỏng thao tác tốn thời gian
	Console.WriteLine("Đã lấy xong dữ liệu.");
}

// Gọi hàm bất đồng bộ
await SimulateDataFetchAsync(); // Cần `await` ở đây vì hàm Main của .NET 6+ đã là async

Console.WriteLine("--- Kết thúc thực hành C# cơ bản ---\n");

app.Run();
