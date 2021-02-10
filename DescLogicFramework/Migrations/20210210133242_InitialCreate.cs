using Microsoft.EntityFrameworkCore.Migrations;

namespace DescLogicFramework.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Expedition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Site = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Core = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Half = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectionTextID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LithologicDescriptions",
                columns: table => new
                {
                    LithologicID = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DescriptionReport = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    DescriptionTab = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    DescriptionGroup = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    DescriptionType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    SectionInfoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LithologicDescriptions", x => x.LithologicID);
                    table.ForeignKey(
                        name: "FK_LithologicDescriptions_Sections_SectionInfoID",
                        column: x => x.SectionInfoID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementDescriptions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LithologicID = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    LithologicSubID = table.Column<int>(type: "int", nullable: true),
                    InstrumentReport = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    InstrumentSystem = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    SectionInfoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDescriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MeasurementDescriptions_Sections_SectionInfoID",
                        column: x => x.SectionInfoID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DescriptionColumnValuePairs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    ColumnName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    LithologicDescriptionLithologicID = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescriptionColumnValuePairs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DescriptionColumnValuePairs_LithologicDescriptions_LithologicDescriptionLithologicID",
                        column: x => x.LithologicDescriptionLithologicID,
                        principalTable: "LithologicDescriptions",
                        principalColumn: "LithologicID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LithologicSubintervals",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LithologicSubID = table.Column<int>(type: "int", nullable: true),
                    LithologicDescriptionLithologicID = table.Column<string>(type: "varchar(50)", nullable: true),
                    SectionInfoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LithologicSubintervals", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LithologicSubintervals_LithologicDescriptions_LithologicDescriptionLithologicID",
                        column: x => x.LithologicDescriptionLithologicID,
                        principalTable: "LithologicDescriptions",
                        principalColumn: "LithologicID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LithologicSubintervals_Sections_SectionInfoID",
                        column: x => x.SectionInfoID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementColumnValuePairs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    ColumnName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    LithologicSubID = table.Column<int>(type: "int", nullable: true),
                    LithologicID = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    MeasurementID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementColumnValuePairs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MeasurementColumnValuePairs_MeasurementDescriptions_MeasurementID",
                        column: x => x.MeasurementID,
                        principalTable: "MeasurementDescriptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LithologicSubintervalMeasurement",
                columns: table => new
                {
                    LithologicSubintervalsID = table.Column<int>(type: "int", nullable: false),
                    MeasurementsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LithologicSubintervalMeasurement", x => new { x.LithologicSubintervalsID, x.MeasurementsID });
                    table.ForeignKey(
                        name: "FK_LithologicSubintervalMeasurement_LithologicSubintervals_LithologicSubintervalsID",
                        column: x => x.LithologicSubintervalsID,
                        principalTable: "LithologicSubintervals",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LithologicSubintervalMeasurement_MeasurementDescriptions_MeasurementsID",
                        column: x => x.MeasurementsID,
                        principalTable: "MeasurementDescriptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionColumnValuePairs_LithologicDescriptionLithologicID",
                table: "DescriptionColumnValuePairs",
                column: "LithologicDescriptionLithologicID");

            migrationBuilder.CreateIndex(
                name: "IX_LithologicDescriptions_SectionInfoID",
                table: "LithologicDescriptions",
                column: "SectionInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_LithologicSubintervalMeasurement_MeasurementsID",
                table: "LithologicSubintervalMeasurement",
                column: "MeasurementsID");

            migrationBuilder.CreateIndex(
                name: "IX_LithologicSubintervals_LithologicDescriptionLithologicID",
                table: "LithologicSubintervals",
                column: "LithologicDescriptionLithologicID");

            migrationBuilder.CreateIndex(
                name: "IX_LithologicSubintervals_SectionInfoID",
                table: "LithologicSubintervals",
                column: "SectionInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementColumnValuePairs_MeasurementID",
                table: "MeasurementColumnValuePairs",
                column: "MeasurementID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDescriptions_SectionInfoID",
                table: "MeasurementDescriptions",
                column: "SectionInfoID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DescriptionColumnValuePairs");

            migrationBuilder.DropTable(
                name: "LithologicSubintervalMeasurement");

            migrationBuilder.DropTable(
                name: "MeasurementColumnValuePairs");

            migrationBuilder.DropTable(
                name: "LithologicSubintervals");

            migrationBuilder.DropTable(
                name: "MeasurementDescriptions");

            migrationBuilder.DropTable(
                name: "LithologicDescriptions");

            migrationBuilder.DropTable(
                name: "Sections");
        }
    }
}
