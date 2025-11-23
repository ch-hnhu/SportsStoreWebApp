using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SportsStore.Domain.Models;
using System.Threading.Tasks;

namespace SportsStore.Infrastructure
{
	public static class AppIdentityDbContextSeed
	{
		public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			string adminRoleName = "Admin";
			string adminEmail = "admin@sportsstore.com";
			string adminPassword = "Admin@123"; // Mật khẩu mạnh, nên dùng biến môi trường hoặc Secret Manager

			// Tạo vai trò "Admin" nếu chưa tồn tại
			if (await roleManager.FindByNameAsync(adminRoleName) == null)
			{
				await roleManager.CreateAsync(new IdentityRole(adminRoleName));
			}
			// Tạo tài khoản Admin nếu chưa tồn tại
			if (await userManager.FindByNameAsync(adminEmail) == null)
			{
				var adminUser = new ApplicationUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					EmailConfirmed = true // Bỏ qua xác nhận email cho admin
				};
				var result = await userManager.CreateAsync(adminUser, adminPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, adminRoleName);
				}
				else
				{
					// Log lỗi nếu tạo admin không thành công
					// Ví dụ: Console.WriteLine(string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}
	}
}