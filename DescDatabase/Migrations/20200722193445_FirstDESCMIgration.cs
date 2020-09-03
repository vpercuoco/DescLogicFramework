using Microsoft.EntityFrameworkCore.Migrations;

namespace DescDatabase.Migrations
{
    public partial class FirstDESCMIgration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Descriptions",
                columns: table => new
                {
                    LithologicID = table.Column<string>(nullable: false),
                    DescriptionReport = table.Column<string>(nullable: true),
                    DescriptionTab = table.Column<string>(nullable: true),
                    DescriptionGroup = table.Column<string>(nullable: true),
                    DescriptionType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descriptions", x => x.LithologicID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Descriptions");
        }
    }
}
