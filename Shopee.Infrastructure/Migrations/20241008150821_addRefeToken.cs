using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRefeToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "6fafdd92-d2da-4499-9bf1-a11f187c98ba");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "aae23d90-e1cc-46bf-91e7-2d8cf35e3a73");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "262fa854-ae4e-4425-a3da-6eeb87506f66", "1", "Admin", "Admin" },
                    { "2e4438a2-7c2c-4289-9c54-9b91ee4d5e4e", "2", "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "262fa854-ae4e-4425-a3da-6eeb87506f66");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "2e4438a2-7c2c-4289-9c54-9b91ee4d5e4e");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6fafdd92-d2da-4499-9bf1-a11f187c98ba", "2", "User", "User" },
                    { "aae23d90-e1cc-46bf-91e7-2d8cf35e3a73", "1", "Admin", "Admin" }
                });
        }
    }
}