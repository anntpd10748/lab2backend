using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServerGame106.Migrations
{
    /// <inheritdoc />
    public partial class Insert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GameLevels",
                columns: new[] { "LevelID", "Description", "title" },
                values: new object[,]
                {
                    { 1, null, "level 1" },
                    { 2, null, "level 2" },
                    { 3, null, "level 3" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "QuestionId", "Answer", "ContentQuestion", "LevelId", "Option1", "Option2", "Option3", "Option4" },
                values: new object[,]
                {
                    { 1, "dap an 1", "cau hoi 1", 1, "dap an 1", "dap an 2", "dap an 3", "dap an 4" },
                    { 2, "dap an 2", "cau hoi 2", 2, "dap an 1", "dap an 2", "dap an 3", "dap an 4" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "RegionId", "RegionName" },
                values: new object[,]
                {
                    { 1, "dong bang song hong" },
                    { 2, "dong bang song cuu long" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GameLevels",
                keyColumn: "LevelID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GameLevels",
                keyColumn: "LevelID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "GameLevels",
                keyColumn: "LevelID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "RegionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "RegionId",
                keyValue: 2);
        }
    }
}
