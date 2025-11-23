using SportsStore.Domain.Models;
using SportsStoreWebApp.Middleware;
using SportsStore.Domain.Abstract;
using SportsStoreWebApp.Concrete;
using SportsStoreWebApp.Configurations;
using Microsoft.EntityFrameworkCore;
using SportsStore.Infrastructure;
using SportsStore.Infrastructure.Repositories; // Namespace của DbContext
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm RazorPages để hỗ trợ Identity UI
builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
builder.Configuration.GetConnectionString("SportsStoreConnection")));

// Cấu hình ASP.NET Core Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	// Cấu hình password
	options.Password.RequireDigit = true; // Yêu cầu có số
	options.Password.RequireLowercase = true; // Yêu cầu chữ thường
	options.Password.RequireUppercase = true; // Yêu cầu chữ hoa
	options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt
	options.Password.RequiredLength = 6; // Độ dài tối thiểu

	// Cấu hình lockout
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	// Cấu hình user
	options.User.RequireUniqueEmail = true;

	// Cấu hình SignIn
	options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// builder.Services.AddScoped<IProductRepository, FakeProductRepository>();

builder.Services.AddScoped<IProductRepository, EFProductRepository>(); // Thay thế bằng EFProductRepository

builder.Services.Configure<PagingSettings>(builder.Configuration.GetSection("PagingSettings"));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Áp dụng Migrations và Seed Data khi ứng dụng khởi động (chỉ cho dev / test)
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<ApplicationDbContext>();
		// Áp dụng tất cả các Migrations chưa áp dụng (nếu có)
		context.Database.Migrate();
		// Seed Identity Roles và User
		await AppIdentityDbContextSeed.SeedRolesAndUsers(services);
		// Seed dữ liệu sản phẩm/danh mục ban đầu (nếu chưa có trong Migration)
		// await SeedData.EnsurePopulated(context);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred seeding the DB.");
	}
}

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

// QUAN TRỌNG: Thứ tự phải đúng
app.UseAuthentication(); // Thêm middleware Authentication
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

// Map Razor Pages cho Identity UI
app.MapRazorPages();

app.Run();
