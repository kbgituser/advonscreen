using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class advertisementPoint3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Heigth",
                table: "Points");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Points",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Points");

            migrationBuilder.AddColumn<int>(
                name: "Heigth",
                table: "Points",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
