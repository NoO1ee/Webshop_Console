using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorityName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authoriries_Users_UserId",
                table: "Authoriries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authoriries",
                table: "Authoriries");

            migrationBuilder.RenameTable(
                name: "Authoriries",
                newName: "Authorities");

            migrationBuilder.RenameIndex(
                name: "IX_Authoriries_UserId",
                table: "Authorities",
                newName: "IX_Authorities_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Authorities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Authorities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authorities",
                table: "Authorities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Authorities_Users_UserId",
                table: "Authorities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authorities_Users_UserId",
                table: "Authorities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authorities",
                table: "Authorities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Authorities");

            migrationBuilder.RenameTable(
                name: "Authorities",
                newName: "Authoriries");

            migrationBuilder.RenameIndex(
                name: "IX_Authorities_UserId",
                table: "Authoriries",
                newName: "IX_Authoriries_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Authoriries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authoriries",
                table: "Authoriries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Authoriries_Users_UserId",
                table: "Authoriries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
