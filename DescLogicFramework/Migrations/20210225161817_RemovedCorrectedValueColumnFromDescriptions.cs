using Microsoft.EntityFrameworkCore.Migrations;

namespace DescLogicFramework.Migrations
{
    public partial class RemovedCorrectedValueColumnFromDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectedValue",
                table: "DescriptionColumnValuePairs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectedValue",
                table: "DescriptionColumnValuePairs",
                type: "varchar(5000)",
                maxLength: 5000,
                nullable: true);
        }
    }
}
