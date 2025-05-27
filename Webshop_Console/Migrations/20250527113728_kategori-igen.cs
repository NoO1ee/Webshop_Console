using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class kategoriigen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_CategoryModel_CategoryId",
                table: "Articles");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Articles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_CategoryModel_CategoryId",
                table: "Articles",
                column: "CategoryId",
                principalTable: "CategoryModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_CategoryModel_CategoryId",
                table: "Articles");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_CategoryModel_CategoryId",
                table: "Articles",
                column: "CategoryId",
                principalTable: "CategoryModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
