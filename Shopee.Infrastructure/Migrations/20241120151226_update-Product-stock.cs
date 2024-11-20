using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateProductstock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "6d72ba1d-a1f2-416b-a159-496106d84483");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "bbc5f3d8-ffa6-4996-9414-ec5fe65811d5");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Product",
                newName: "Quantity");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "340799f5-4189-4672-a421-dbb72b69489e", "2", "User", "User" },
                    { "ddfd5c2d-062f-4098-ae91-d88a3f7e4b93", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "340799f5-4189-4672-a421-dbb72b69489e");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "ddfd5c2d-062f-4098-ae91-d88a3f7e4b93");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Product",
                newName: "Stock");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6d72ba1d-a1f2-416b-a159-496106d84483", "2", "User", "User" },
                    { "bbc5f3d8-ffa6-4996-9414-ec5fe65811d5", "1", "Admin", "Admin" }
                });
        }
    }
}
