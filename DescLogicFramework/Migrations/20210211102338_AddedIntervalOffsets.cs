using Microsoft.EntityFrameworkCore.Migrations;

namespace DescLogicFramework.Migrations
{
    public partial class AddedIntervalOffsets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EndOffset",
                table: "MeasurementDescriptions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartOffset",
                table: "MeasurementDescriptions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EndOffset",
                table: "LithologicSubintervals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartOffset",
                table: "LithologicSubintervals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EndOffset",
                table: "LithologicDescriptions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartOffset",
                table: "LithologicDescriptions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndOffset",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "StartOffset",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "EndOffset",
                table: "LithologicSubintervals");

            migrationBuilder.DropColumn(
                name: "StartOffset",
                table: "LithologicSubintervals");

            migrationBuilder.DropColumn(
                name: "EndOffset",
                table: "LithologicDescriptions");

            migrationBuilder.DropColumn(
                name: "StartOffset",
                table: "LithologicDescriptions");
        }
    }
}
