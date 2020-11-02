using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class LengthToWidth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Points");

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Points",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Width",
                table: "Points");

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Points",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
