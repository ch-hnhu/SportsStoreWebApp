// SportsStore.Tests/CartTests.cs
using Xunit;
using SportsStore.Domain.Models; // Namespace của Cart và Product
using System.Linq;
public class CartTests
{
	[Fact] // Một test case riêng lẻ
	public void Can_Add_New_Items()
	{
		// Arrange - Chuẩn bị dữ liệu và đối tượng
		Product p1 = new Product
		{
			ProductID = 1,
			Name = "P1",
			Price =
	   10m
		};
		Product p2 = new Product
		{
			ProductID = 2,
			Name = "P2",
			Price =
	   20m
		};
		Cart target = new Cart();
		// Act - Thực hiện hành động
		target.AddItem(p1, 1);
		target.AddItem(p2, 1);
		CartLine[] results = target.Items.ToArray();
		// Assert - Kiểm tra kết quả
		Assert.Equal(2, results.Length);
		Assert.Equal(p1, results[0].Product);
		Assert.Equal(p2, results[1].Product);
	}
	[Fact]
	public void Can_Add_Quantity_For_Existing_Items()
	{
		// Arrange
		Product p1 = new Product
		{
			ProductID = 1,
			Name = "P1",
			Price =
	   10m
		};
		Product p2 = new Product
		{
			ProductID = 2,
			Name = "P2",
			Price = 20m
		};
		Cart target = new Cart();
		// Act
		target.AddItem(p1, 1);
		target.AddItem(p2, 1);
		target.AddItem(p1, 10); // Thêm số lượng cho P1
		CartLine[] results = target.Items.OrderBy(c => c.Product.ProductID).ToArray();
		// Assert
		Assert.Equal(2, results.Length); // Vẫn chỉ có 2 loại sản phẩm
		Assert.Equal(11, results[0].Quantity); // P1 có 11
		Assert.Equal(1, results[1].Quantity); // P2 có 1
	}
	[Fact]
	public void Can_Remove_Item()
	{
		// Arrange
		Product p1 = new Product
		{
			ProductID = 1,
			Name = "P1",
			Price = 10m
		};
		Product p2 = new Product
		{
			ProductID = 2,
			Name = "P2",
			Price = 20m
		};
		Product p3 = new Product
		{
			ProductID = 3,
			Name = "P3",
			Price = 30m
		};
		Cart target = new Cart();
		target.AddItem(p1, 1);
	}
}