using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class kategoriigen3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CategoryModel",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Legendarisk" },
                    { 2, "Episk" },
                    { 3, "Basic" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CategoryModel",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CategoryModel",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CategoryModel",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
