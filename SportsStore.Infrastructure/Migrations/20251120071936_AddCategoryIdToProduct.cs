using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStore.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class AddCategoryIdToProduct : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "Products",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(100)",
				oldMaxLength: 100);

			migrationBuilder.AlterColumn<string>(
				name: "Description",
				table: "Products",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(500)",
				oldMaxLength: 500);

			migrationBuilder.AddColumn<int>(
				name: "CategoryId",
				table: "Products",
				type: "int",
				nullable: false,
				defaultValue: 1);

			// Update existing products based on their Category string values
			migrationBuilder.Sql(@"
                UPDATE Products SET CategoryId = 1 WHERE Category = N'Bóng đá';
                UPDATE Products SET CategoryId = 2 WHERE Category = N'Cờ vua';
                UPDATE Products SET CategoryId = 3 WHERE Category = N'Bóng chuyền';
            ");

			migrationBuilder.CreateIndex(
				name: "IX_Products_CategoryId",
				table: "Products",
				column: "CategoryId");

			migrationBuilder.AddForeignKey(
				name: "FK_Products_Categories_CategoryId",
				table: "Products",
				column: "CategoryId",
				principalTable: "Categories",
				principalColumn: "CategoryID",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Products_Categories_CategoryId",
				table: "Products");

			migrationBuilder.DropIndex(
				name: "IX_Products_CategoryId",
				table: "Products");

			migrationBuilder.DropColumn(
				name: "CategoryId",
				table: "Products");

			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "Products",
				type: "nvarchar(100)",
				maxLength: 100,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			migrationBuilder.AlterColumn<string>(
				name: "Description",
				table: "Products",
				type: "nvarchar(500)",
				maxLength: 500,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");
		}
	}
}
