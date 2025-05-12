using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class Roller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authorities",
                columns: new[] { "Id", "IsAdmin", "IsOwner", "Name" },
                values: new object[,]
                {
                    { 1, false, false, "User" },
                    { 2, true, false, "Admin" },
                    { 3, true, true, "Owner" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authorities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authorities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authorities",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
