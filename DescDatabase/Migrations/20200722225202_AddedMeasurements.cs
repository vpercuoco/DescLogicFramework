using Microsoft.EntityFrameworkCore.Migrations;

namespace DescDatabase.Migrations
{
    public partial class AddedMeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Descriptions",
                table: "Descriptions");

            migrationBuilder.RenameTable(
                name: "Descriptions",
                newName: "LithologicDescriptions");

            migrationBuilder.AddColumn<int>(
                name: "SectionInfoSectionTextID",
                table: "LithologicDescriptions",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions",
                column: "LithologicID");

            migrationBuilder.CreateTable(
                name: "SectionInfo",
                columns: table => new
                {
                    SectionTextID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Expedition = table.Column<string>(nullable: true),
                    Site = table.Column<string>(nullable: true),
                    Hole = table.Column<string>(nullable: true),
                    Core = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Section = table.Column<string>(nullable: true),
                    SampleID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionInfo", x => x.SectionTextID);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementDescriptions",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionInfoSectionTextID = table.Column<int>(nullable: true),
                    InstrumentReport = table.Column<string>(nullable: true),
                    InstrumentSystem = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDescriptions", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_MeasurementDescriptions_SectionInfo_SectionInfoSectionTextID",
                        column: x => x.SectionInfoSectionTextID,
                        principalTable: "SectionInfo",
                        principalColumn: "SectionTextID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LithologicDescriptions_SectionInfoSectionTextID",
                table: "LithologicDescriptions",
                column: "SectionInfoSectionTextID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDescriptions_SectionInfoSectionTextID",
                table: "MeasurementDescriptions",
                column: "SectionInfoSectionTextID");

            migrationBuilder.AddForeignKey(
                name: "FK_LithologicDescriptions_SectionInfo_SectionInfoSectionTextID",
                table: "LithologicDescriptions",
                column: "SectionInfoSectionTextID",
                principalTable: "SectionInfo",
                principalColumn: "SectionTextID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LithologicDescriptions_SectionInfo_SectionInfoSectionTextID",
                table: "LithologicDescriptions");

            migrationBuilder.DropTable(
                name: "MeasurementDescriptions");

            migrationBuilder.DropTable(
                name: "SectionInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_LithologicDescriptions_SectionInfoSectionTextID",
                table: "LithologicDescriptions");

            migrationBuilder.DropColumn(
                name: "SectionInfoSectionTextID",
                table: "LithologicDescriptions");

            migrationBuilder.RenameTable(
                name: "LithologicDescriptions",
                newName: "Descriptions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Descriptions",
                table: "Descriptions",
                column: "LithologicID");
        }
    }
}
