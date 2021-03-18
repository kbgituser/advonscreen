using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class photovideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUrl",
                table: "Advertisements");

            migrationBuilder.AddColumn<int>(
                name: "AdvertisementType",
                table: "Advertisements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Advertisements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "Advertisements",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvertisementType",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "Advertisements");

            migrationBuilder.AddColumn<string>(
                name: "DataUrl",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
