using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeTableRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "2401723c-c4b0-4232-ac70-987c7c402dab");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "9b829d7d-9023-4a1b-b846-4e64e4374be0");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "RefreshTokens",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1498bb27-e725-4cfa-9a08-0c73f14b12f7", "2", "User", "User" },
                    { "e28ec7c4-7f19-4afd-9a71-b4b1d3320fd4", "1", "Admin", "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Id",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_Id",
                table: "RefreshTokens",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_Id",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Id",
                table: "RefreshTokens");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "1498bb27-e725-4cfa-9a08-0c73f14b12f7");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e28ec7c4-7f19-4afd-9a71-b4b1d3320fd4");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RefreshTokens");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2401723c-c4b0-4232-ac70-987c7c402dab", "2", "User", "User" },
                    { "9b829d7d-9023-4a1b-b846-4e64e4374be0", "1", "Admin", "Admin" }
                });
        }
    }
}