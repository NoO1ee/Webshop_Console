using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class Moms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Net",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Vat",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Net",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "Orders");
        }
    }
}
