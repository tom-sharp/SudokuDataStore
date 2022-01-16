using Microsoft.EntityFrameworkCore.Migrations;

namespace Sudoku.DataStore.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SudokuPuzzles",
                columns: table => new
                {
                    PuzzleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Clues = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Validated = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SudokuPuzzles", x => x.PuzzleId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SudokuPuzzles");
        }
    }
}
