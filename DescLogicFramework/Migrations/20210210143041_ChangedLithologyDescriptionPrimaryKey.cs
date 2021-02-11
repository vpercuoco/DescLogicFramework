using Microsoft.EntityFrameworkCore.Migrations;

namespace DescLogicFramework.Migrations
{
    public partial class ChangedLithologyDescriptionPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionColumnValuePairs_LithologicDescriptions_LithologicDescriptionLithologicID",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.DropForeignKey(
                name: "FK_LithologicSubintervals_LithologicDescriptions_LithologicDescriptionLithologicID",
                table: "LithologicSubintervals");

            migrationBuilder.DropIndex(
                name: "IX_LithologicSubintervals_LithologicDescriptionLithologicID",
                table: "LithologicSubintervals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_DescriptionColumnValuePairs_LithologicDescriptionLithologicID",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.DropColumn(
                name: "LithologicDescriptionLithologicID",
                table: "LithologicSubintervals");

            migrationBuilder.DropColumn(
                name: "LithologicDescriptionLithologicID",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.AddColumn<int>(
                name: "LithologicDescriptionID",
                table: "LithologicSubintervals",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LithologicID",
                table: "LithologicDescriptions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "LithologicDescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "LithologicDescriptionID",
                table: "DescriptionColumnValuePairs",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_LithologicSubintervals_LithologicDescriptionID",
                table: "LithologicSubintervals",
                column: "LithologicDescriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionColumnValuePairs_LithologicDescriptionID",
                table: "DescriptionColumnValuePairs",
                column: "LithologicDescriptionID");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionColumnValuePairs_LithologicDescriptions_LithologicDescriptionID",
                table: "DescriptionColumnValuePairs",
                column: "LithologicDescriptionID",
                principalTable: "LithologicDescriptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LithologicSubintervals_LithologicDescriptions_LithologicDescriptionID",
                table: "LithologicSubintervals",
                column: "LithologicDescriptionID",
                principalTable: "LithologicDescriptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionColumnValuePairs_LithologicDescriptions_LithologicDescriptionID",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.DropForeignKey(
                name: "FK_LithologicSubintervals_LithologicDescriptions_LithologicDescriptionID",
                table: "LithologicSubintervals");

            migrationBuilder.DropIndex(
                name: "IX_LithologicSubintervals_LithologicDescriptionID",
                table: "LithologicSubintervals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_DescriptionColumnValuePairs_LithologicDescriptionID",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.DropColumn(
                name: "LithologicDescriptionID",
                table: "LithologicSubintervals");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "LithologicDescriptions");

            migrationBuilder.DropColumn(
                name: "LithologicDescriptionID",
                table: "DescriptionColumnValuePairs");

            migrationBuilder.AddColumn<string>(
                name: "LithologicDescriptionLithologicID",
                table: "LithologicSubintervals",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LithologicID",
                table: "LithologicDescriptions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LithologicDescriptionLithologicID",
                table: "DescriptionColumnValuePairs",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LithologicDescriptions",
                table: "LithologicDescriptions",
                column: "LithologicID");

            migrationBuilder.CreateIndex(
                name: "IX_LithologicSubintervals_LithologicDescriptionLithologicID",
                table: "LithologicSubintervals",
                column: "LithologicDescriptionLithologicID");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionColumnValuePairs_LithologicDescriptionLithologicID",
                table: "DescriptionColumnValuePairs",
                column: "LithologicDescriptionLithologicID");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionColumnValuePairs_LithologicDescriptions_LithologicDescriptionLithologicID",
                table: "DescriptionColumnValuePairs",
                column: "LithologicDescriptionLithologicID",
                principalTable: "LithologicDescriptions",
                principalColumn: "LithologicID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LithologicSubintervals_LithologicDescriptions_LithologicDescriptionLithologicID",
                table: "LithologicSubintervals",
                column: "LithologicDescriptionLithologicID",
                principalTable: "LithologicDescriptions",
                principalColumn: "LithologicID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
