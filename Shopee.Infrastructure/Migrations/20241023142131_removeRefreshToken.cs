using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "1498bb27-e725-4cfa-9a08-0c73f14b12f7");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e28ec7c4-7f19-4afd-9a71-b4b1d3320fd4");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "e72ed4dc-89d4-4152-b414-3dca638e7c97", "1", "Admin", "Admin" },
                    { "ed903a96-9ffc-4371-b537-d65172a3650e", "2", "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e72ed4dc-89d4-4152-b414-3dca638e7c97");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "ed903a96-9ffc-4371-b537-d65172a3650e");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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
        }
    }
}