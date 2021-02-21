using Microsoft.EntityFrameworkCore.Migrations;

namespace DescLogicFramework.Migrations
{
    public partial class AddedTextIDandTestNumberToMeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestNumber",
                table: "MeasurementDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextID",
                table: "MeasurementDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestNumber",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "TextID",
                table: "MeasurementDescriptions");
        }
    }
}
