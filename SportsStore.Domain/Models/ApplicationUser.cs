using Microsoft.AspNetCore.Identity;

namespace SportsStore.Domain.Models
{
	/// <summary>
	/// Custom User class kế thừa từ IdentityUser
	/// Có thể thêm các thuộc tính tùy chỉnh nếu cần
	/// </summary>
	public class ApplicationUser : IdentityUser
	{
		// Có thể thêm các thuộc tính tùy chỉnh ở đây
		// Ví dụ:
		// public string? FullName { get; set; }
		// public DateTime? DateOfBirth { get; set; }
	}
}
