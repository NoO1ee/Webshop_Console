using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Webshop_Console.Migrations
{
    /// <inheritdoc />
    public partial class kategoriigen5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "ArticleCode", "Bio", "CategoryId", "DiscountId", "EanCode", "IsFeatured", "Name", "Price", "Storage", "SupplierId", "UnitId" },
                values: new object[,]
                {
                    { 1, "344233", "Skuggornas mördare - osynlig och snabb", 3, 1, "30291823", false, "Quenton Quack", 3000m, 200, 1, 1 },
                    { 2, "430233", "Mästarens prickskytt - ser från mils avstånd", 1, 1, "20230304", false, "Beatrix Drake", 50000m, 2, 1, 1 },
                    { 3, "414663", "Spion-anka - infiltrerar fiendelinjer", 3, 1, "23232123", true, "Agent Mallory Mallard", 4499m, 42, 1, 1 },
                    { 4, "748454", "Skogens väktare - bågskytt och överlevare", 2, 1, "12312312", false, "Archibald Quill", 15000m, 32, 1, 1 },
                    { 5, "112577", "Elementarkrigare - kontrollerar eld och is", 2, 1, "44512315", true, "Magnus Mallard", 12999m, 15, 1, 1 },
                    { 6, "245552", "Helande räddare - botar och stärker allierade", 3, 1, "04912454", false, "Helga Mallard", 5600m, 16, 1, 1 },
                    { 7, "692042", "Oövervinnerlig - drar åt sig all fiendeuppmärksamhet", 3, 1, "07669964", false, "Bruno Beak", 4420m, 6, 1, 1 },
                    { 8, "024132", "Mekaniker-anka - bygger fällor och turrets", 2, 1, "50042244", false, "Greta Quack", 11000m, 4, 1, 1 },
                    { 9, "400234", "Ismagikern - fryser fiender på plats", 1, 1, "12499902", false, "Finn Feather", 90300m, 1, 1, 1 },
                    { 10, "932491", "Vildsint kämpe - kullkastar allt som står i vägen", 1, 1, "29490501", false, "Roland Drake", 159000m, 3, 1, 1 },
                    { 11, "233993", "Helig präst - välsignar och fördriver mörker", 3, 1, "91204329", false, "Lyra Quill", 1500m, 1, 1, 1 },
                    { 12, "499532", "Listig tjuv - snor skatter och smyger förbi vakter", 3, 1, "49402123", false, "Shadow Mallard", 4030m, 3, 1, 1 },
                    { 13, "230032", "Test Anka", 2, 1, "590239583", false, "Fighter Ducker", 4223m, 2, 1, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 13);
        }
    }
}
