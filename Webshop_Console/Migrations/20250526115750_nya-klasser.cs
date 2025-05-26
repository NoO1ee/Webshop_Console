using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class nyaklasser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorityUser");

            migrationBuilder.CreateTable(
                name: "AuthorityModelUserModel",
                columns: table => new
                {
                    AuthoritiesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorityModelUserModel", x => new { x.AuthoritiesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AuthorityModelUserModel_Authorities_AuthoritiesId",
                        column: x => x.AuthoritiesId,
                        principalTable: "Authorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorityModelUserModel_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorityModelUserModel_UsersId",
                table: "AuthorityModelUserModel",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorityModelUserModel");

            migrationBuilder.CreateTable(
                name: "AuthorityUser",
                columns: table => new
                {
                    AuthoritiesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorityUser", x => new { x.AuthoritiesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AuthorityUser_Authorities_AuthoritiesId",
                        column: x => x.AuthoritiesId,
                        principalTable: "Authorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorityUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorityUser_UsersId",
                table: "AuthorityUser",
                column: "UsersId");
        }
    }
}
