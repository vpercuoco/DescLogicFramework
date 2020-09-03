using Microsoft.EntityFrameworkCore.Migrations;

namespace DescDatabase.Migrations
{
    public partial class AddedSectionInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LithologicDescriptions_SectionInfo_SectionInfoSectionTextID",
                table: "LithologicDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementDescriptions_SectionInfo_SectionInfoSectionTextID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectionInfo",
                table: "SectionInfo");

            migrationBuilder.RenameTable(
                name: "SectionInfo",
                newName: "Sections");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sections",
                table: "Sections",
                column: "SectionTextID");

            migrationBuilder.AddForeignKey(
                name: "FK_LithologicDescriptions_Sections_SectionInfoSectionTextID",
                table: "LithologicDescriptions",
                column: "SectionInfoSectionTextID",
                principalTable: "Sections",
                principalColumn: "SectionTextID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDescriptions_Sections_SectionInfoSectionTextID",
                table: "MeasurementDescriptions",
                column: "SectionInfoSectionTextID",
                principalTable: "Sections",
                principalColumn: "SectionTextID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LithologicDescriptions_Sections_SectionInfoSectionTextID",
                table: "LithologicDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementDescriptions_Sections_SectionInfoSectionTextID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sections",
                table: "Sections");

            migrationBuilder.RenameTable(
                name: "Sections",
                newName: "SectionInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectionInfo",
                table: "SectionInfo",
                column: "SectionTextID");

            migrationBuilder.AddForeignKey(
                name: "FK_LithologicDescriptions_SectionInfo_SectionInfoSectionTextID",
                table: "LithologicDescriptions",
                column: "SectionInfoSectionTextID",
                principalTable: "SectionInfo",
                principalColumn: "SectionTextID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDescriptions_SectionInfo_SectionInfoSectionTextID",
                table: "MeasurementDescriptions",
                column: "SectionInfoSectionTextID",
                principalTable: "SectionInfo",
                principalColumn: "SectionTextID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
