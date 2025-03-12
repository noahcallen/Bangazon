using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bangazon.Migrations
{
    /// <inheritdoc />
    public partial class users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "SellerId",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Image", "Name", "Price", "Quantity", "SellerId" },
                values: new object[] { "Compact and fuel-efficient sedan.", "civic.jpg", "Honda Civic", 24000.99m, 8, "user_1" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Image", "Name", "Price", "Quantity", "SellerId" },
                values: new object[] { "Reliable and affordable compact sedan.", "corolla.jpg", "Toyota Corolla", 22000.99m, 10, "user_2" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "Image", "Name", "Price", "SellerId" },
                values: new object[] { 1, "High-performance luxury electric sedan.", "models.jpg", "Tesla Model S", 79999.99m, "user_3" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Image", "Name", "Price", "Quantity", "SellerId" },
                values: new object[] { "Spacious SUV for family trips.", "explorer.jpg", "Ford Explorer", 35000.99m, 5, "user_1" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Image", "Name", "Price", "Quantity", "SellerId" },
                values: new object[] { 2, "Full-size SUV with premium features.", "tahoe.jpg", "Chevrolet Tahoe", 48000.99m, 4, "user_2" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "Image", "Name", "Price", "Quantity", "SellerId" },
                values: new object[] { 2, "Off-road capable and luxury interior.", "grandcherokee.jpg", "Jeep Grand Cherokee", 42000.99m, 7, "user_3" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Image", "IsAvailable", "Name", "Price", "Quantity", "SellerId" },
                values: new object[,]
                {
                    { 7, 3, "Powerful truck for all your needs.", "f150.jpg", true, "Ford F-150", 42000.99m, 6, "user_1" },
                    { 8, 3, "Durable truck with a smooth ride.", "ram1500.jpg", true, "Ram 1500", 45000.99m, 5, "user_2" },
                    { 9, 3, "Strong towing capacity and reliability.", "silverado.jpg", true, "Chevrolet Silverado", 46000.99m, 8, "user_3" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SellerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Image", "Name", "Price", "Quantity", "StoreId" },
                values: new object[] { "Reliable and fuel-efficient sedan.", "camry.jpg", "Toyota Camry", 25000.99m, 5, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Image", "Name", "Price", "Quantity", "StoreId" },
                values: new object[] { "Spacious and comfortable sedan.", "accord.jpg", "Honda Accord", 27000.99m, 4, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "Image", "Name", "Price", "StoreId" },
                values: new object[] { 2, "Spacious SUV for family trips.", "explorer.jpg", "Ford Explorer", 35000.99m, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Image", "Name", "Price", "Quantity", "StoreId" },
                values: new object[] { "Full-size SUV with premium features.", "tahoe.jpg", "Chevrolet Tahoe", 48000.99m, 2, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Image", "Name", "Price", "Quantity", "StoreId" },
                values: new object[] { 3, "Powerful truck for all your needs.", "f150.jpg", "Ford F-150", 42000.99m, 6, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "Image", "Name", "Price", "Quantity", "StoreId" },
                values: new object[] { 3, "Durable truck with a smooth ride.", "ram1500.jpg", "Ram 1500", 45000.99m, 3, 2 });

            migrationBuilder.InsertData(
                table: "Store",
                columns: new[] { "Id", "Name", "SellerId" },
                values: new object[,]
                {
                    { 1, "Johnson's Auto Sales", "user_1" },
                    { 2, "Davis Car Dealership", "user_2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");
        }
    }
}
