using Microsoft.EntityFrameworkCore.Migrations;

namespace DescLogicFramework.Migrations
{
    public partial class UnmappedCertainFieldsFromDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LithologicID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "LithologicSubID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "DescriptionGroup",
                table: "LithologicDescriptions");

            migrationBuilder.DropColumn(
                name: "DescriptionTab",
                table: "LithologicDescriptions");

            migrationBuilder.DropColumn(
                name: "DescriptionType",
                table: "LithologicDescriptions");

            migrationBuilder.DropColumn(
                name: "LithologicID",
                table: "LithologicDescriptions");

            migrationBuilder.AddColumn<string>(
                name: "CorrectedColumnName",
                table: "DescriptionColumnValuePairs",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectedValue",
                table: "DescriptionColumnValuePairs",
                type: "varchar(5000)",
                maxLength: 5000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectedColumnName",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.DropColumn(
                name: "CorrectedValue",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.AddColumn<string>(
                name: "LithologicID",
                table: "MeasurementDescriptions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LithologicSubID",
                table: "MeasurementDescriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionGroup",
                table: "LithologicDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionTab",
                table: "LithologicDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionType",
                table: "LithologicDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LithologicID",
                table: "LithologicDescriptions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
