// SportsStore.Tests/EFProductRepositoryTests.cs
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; // Để dùng ForEachAsync, ToListAsync etc.
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using SportsStore.Infrastructure.Data;
using SportsStore.Infrastructure.Repositories; // Lớp cần test
public class EFProductRepositoryTests
{
	[Fact]
	public async Task Can_Save_New_Product()
	{
		// Arrange
		var mockSet = new Mock<DbSet<Product>>();
		var mockContext = new Mock<ApplicationDbContext>();
		mockContext.Setup(m => m.Products).Returns(mockSet.Object);
		EFProductRepository repo = new EFProductRepository(mockContext.Object);
		Product newProduct = new Product
		{
			ProductID = 0,
			Name = "Test Product",
			Description = "Desc",
			Price = 10,
			Category = "Test"
		};
		// Act
		await repo.SaveProduct(newProduct);
		// Assert
		mockSet.Verify(m => m.Add(It.IsAny<Product>()), Times.Once());
		mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
	}
	[Fact]
	public async Task Can_Update_Existing_Product()
	{
		// Arrange
		Product p1 = new Product { ProductID = 1, Name = "P1" };
		Product p2 = new Product { ProductID = 2, Name = "P2" };
		List<Product> products = new List<Product> { p1, p2 };
		// Setup mock DbSet để giả lập truy vấn
		var mockSet = new Mock<DbSet<Product>>();
		mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.AsQueryable().Provider);
		mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
		mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
		mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());
		// Mock Find/FirstOrDefault cho DbSet
		mockSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Expression<Func<Product, bool>> predicate, CancellationToken token) => products.AsQueryable().FirstOrDefault(predicate));
		var mockContext = new Mock<ApplicationDbContext>();
		mockContext.Setup(m => m.Products).Returns(mockSet.Object);
		EFProductRepository repo = new EFProductRepository(mockContext.Object);
		// Product cần update (có ID)
		Product updatedProduct = new Product
		{
			ProductID = 1,
			Name = "Updated P1",
			Description = "New Desc",
			Price = 15,
			Category = "Updated Cat"
		};
		// Act
		await repo.SaveProduct(updatedProduct);
		// Assert
		Assert.Equal("Updated P1", p1.Name); // Kiểm tra xem đối tượng gốc đã được cập nhật chưa

		mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
		mockSet.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
		// Đảm bảo không gọi Add
	}
	[Fact]
	public async Task Can_Delete_Product()
	{
		// Arrange
		Product p1 = new Product { ProductID = 1, Name = "P1" };
		List<Product> products = new List<Product> { p1 };
		var mockSet = new Mock<DbSet<Product>>();
		mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.AsQueryable().Provider);
		mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
		mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
		mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());
		mockSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
		.ReturnsAsync((Expression<Func<Product, bool>> predicate, CancellationToken token) => products.AsQueryable().FirstOrDefault(predicate));
		var mockContext = new Mock<ApplicationDbContext>();
		mockContext.Setup(m => m.Products).Returns(mockSet.Object);
		EFProductRepository repo = new EFProductRepository(mockContext.Object);
		// Act
		Product? deletedProduct = await repo.DeleteProduct(1);
		// Assert
		Assert.Equal(p1, deletedProduct);
		mockSet.Verify(m => m.Remove(It.IsAny<Product>()), Times.Once());
		mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
	}
}