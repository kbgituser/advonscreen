using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class advertisementPoint2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Heigth",
                table: "Points",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Points",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedFontSize",
                table: "Points",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FontSize",
                table: "Advertisements",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Heigth",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "RecommendedFontSize",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "Advertisements");
        }
    }
}
