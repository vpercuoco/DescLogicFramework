using Microsoft.EntityFrameworkCore.Migrations;

namespace DescLogicFramework.Migrations
{
    public partial class AddedPropertiesToSectionInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionTextID",
                table: "Sections");

            migrationBuilder.RenameColumn(
                name: "Parent",
                table: "Sections",
                newName: "WorkingTextID");

            migrationBuilder.AddColumn<string>(
                name: "ArchiveTextID",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentTextID",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchiveTextID",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ParentTextID",
                table: "Sections");

            migrationBuilder.RenameColumn(
                name: "WorkingTextID",
                table: "Sections",
                newName: "Parent");

            migrationBuilder.AddColumn<int>(
                name: "SectionTextID",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
