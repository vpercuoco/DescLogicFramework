using Microsoft.EntityFrameworkCore.Migrations;

namespace DescDatabase.Migrations
{
    public partial class RefinedDataPropertyinLithologicDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColumnValuePair",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ColumnName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    LithologicDescriptionLithologicID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnValuePair", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ColumnValuePair_LithologicDescriptions_LithologicDescriptionLithologicID",
                        column: x => x.LithologicDescriptionLithologicID,
                        principalTable: "LithologicDescriptions",
                        principalColumn: "LithologicID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnValuePair_LithologicDescriptionLithologicID",
                table: "ColumnValuePair",
                column: "LithologicDescriptionLithologicID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnValuePair");
        }
    }
}
