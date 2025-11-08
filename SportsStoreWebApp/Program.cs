using SportsStore.Domain.Models;
using SportsStoreWebApp.Middleware;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Concrete;
using SportsStoreWebApp.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductRepository, FakeProductRepository>();

builder.Services.Configure<PagingSettings>(builder.Configuration.GetSection("PagingSettings"));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseMiddleware<RequestLoggerMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage(); // Trang lỗi chi tiết cho dev
}
else
{
	app.UseExceptionHandler("/Home/Error"); // Chuyển hướng đến trang lỗi tùy chỉnh
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();


app.MapControllerRoute(
	name: "category_page",
	pattern: "{category}/Page{productPage:int}",
	defaults: new { controller = "Product", action = "List" }
);

app.MapControllerRoute(
	name: "pagination",
	pattern: "Page{productPage:int}",
	defaults: new { controller = "Product", action = "List" }
);

app.MapControllerRoute(
	name: "category",
	pattern: "{category}",
	defaults: new { controller = "Product", action = "List", productPage = 1 }
);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Product}/{action=List}/{id?}"
);

app.Run();
