using Microsoft.EntityFrameworkCore.Migrations;

namespace DescDatabase.Migrations
{
    public partial class AddedSubIntervals_RefinedDataAnnotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColumnValuePair_LithologicDescriptions_LithologicDescriptionLithologicID",
                table: "ColumnValuePair");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeasurementDescriptions",
                table: "MeasurementDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_ColumnValuePair_LithologicDescriptionLithologicID",
                table: "ColumnValuePair");

            migrationBuilder.DropColumn(
                name: "PrimaryKey",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "LithologicDescriptionLithologicID",
                table: "ColumnValuePair");

            migrationBuilder.AlterColumn<string>(
                name: "InstrumentSystem",
                table: "MeasurementDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InstrumentReport",
                table: "MeasurementDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "MeasurementDescriptions",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "LithologicDescriptionID",
                table: "MeasurementDescriptions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LithologicSubintervalLithologicSubID",
                table: "MeasurementDescriptions",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionType",
                table: "LithologicDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionTab",
                table: "LithologicDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionReport",
                table: "LithologicDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionGroup",
                table: "LithologicDescriptions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LithologicID",
                table: "LithologicDescriptions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "LithologicDescriptions",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "LithologicDescriptionID",
                table: "ColumnValuePair",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeasurementDescriptions",
                table: "MeasurementDescriptions",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "LithologicSubinterval",
                columns: table => new
                {
                    LithologicSubID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionInfoSectionTextID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LithologicSubinterval", x => x.LithologicSubID);
                    table.ForeignKey(
                        name: "FK_LithologicSubinterval_Sections_SectionInfoSectionTextID",
                        column: x => x.SectionInfoSectionTextID,
                        principalTable: "Sections",
                        principalColumn: "SectionTextID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDescriptions_LithologicDescriptionID",
                table: "MeasurementDescriptions",
                column: "LithologicDescriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDescriptions_LithologicSubintervalLithologicSubID",
                table: "MeasurementDescriptions",
                column: "LithologicSubintervalLithologicSubID");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnValuePair_LithologicDescriptionID",
                table: "ColumnValuePair",
                column: "LithologicDescriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_LithologicSubinterval_SectionInfoSectionTextID",
                table: "LithologicSubinterval",
                column: "SectionInfoSectionTextID");

            migrationBuilder.AddForeignKey(
                name: "FK_ColumnValuePair_LithologicDescriptions_LithologicDescriptionID",
                table: "ColumnValuePair",
                column: "LithologicDescriptionID",
                principalTable: "LithologicDescriptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDescriptions_LithologicDescriptions_LithologicDescriptionID",
                table: "MeasurementDescriptions",
                column: "LithologicDescriptionID",
                principalTable: "LithologicDescriptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDescriptions_LithologicSubinterval_LithologicSubintervalLithologicSubID",
                table: "MeasurementDescriptions",
                column: "LithologicSubintervalLithologicSubID",
                principalTable: "LithologicSubinterval",
                principalColumn: "LithologicSubID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColumnValuePair_LithologicDescriptions_LithologicDescriptionID",
                table: "ColumnValuePair");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementDescriptions_LithologicDescriptions_LithologicDescriptionID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementDescriptions_LithologicSubinterval_LithologicSubintervalLithologicSubID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropTable(
                name: "LithologicSubinterval");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeasurementDescriptions",
                table: "MeasurementDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementDescriptions_LithologicDescriptionID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementDescriptions_LithologicSubintervalLithologicSubID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_ColumnValuePair_LithologicDescriptionID",
                table: "ColumnValuePair");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "LithologicDescriptionID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "LithologicSubintervalLithologicSubID",
                table: "MeasurementDescriptions");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "LithologicDescriptions");

            migrationBuilder.DropColumn(
                name: "LithologicDescriptionID",
                table: "ColumnValuePair");

            migrationBuilder.AlterColumn<string>(
                name: "InstrumentSystem",
                table: "MeasurementDescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InstrumentReport",
                table: "MeasurementDescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryKey",
                table: "MeasurementDescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "LithologicID",
                table: "LithologicDescriptions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionType",
                table: "LithologicDescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionTab",
                table: "LithologicDescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionReport",
                table: "LithologicDescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionGroup",
                table: "LithologicDescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LithologicDescriptionLithologicID",
                table: "ColumnValuePair",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeasurementDescriptions",
                table: "MeasurementDescriptions",
                column: "PrimaryKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions",
                column: "LithologicID");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnValuePair_LithologicDescriptionLithologicID",
                table: "ColumnValuePair",
                column: "LithologicDescriptionLithologicID");

            migrationBuilder.AddForeignKey(
                name: "FK_ColumnValuePair_LithologicDescriptions_LithologicDescriptionLithologicID",
                table: "ColumnValuePair",
                column: "LithologicDescriptionLithologicID",
                principalTable: "LithologicDescriptions",
                principalColumn: "LithologicID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
