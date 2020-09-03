using Microsoft.EntityFrameworkCore.Migrations;

namespace DescDatabase.Migrations
{
    public partial class AddedMeasurementDataList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeasurementID",
                table: "ColumnValuePair",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColumnValuePair_MeasurementID",
                table: "ColumnValuePair",
                column: "MeasurementID");

            migrationBuilder.AddForeignKey(
                name: "FK_ColumnValuePair_MeasurementDescriptions_MeasurementID",
                table: "ColumnValuePair",
                column: "MeasurementID",
                principalTable: "MeasurementDescriptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColumnValuePair_MeasurementDescriptions_MeasurementID",
                table: "ColumnValuePair");

            migrationBuilder.DropIndex(
                name: "IX_ColumnValuePair_MeasurementID",
                table: "ColumnValuePair");

            migrationBuilder.DropColumn(
                name: "MeasurementID",
                table: "ColumnValuePair");
        }
    }
}
