using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class LäggTillFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Articles");
        }
    }
}
