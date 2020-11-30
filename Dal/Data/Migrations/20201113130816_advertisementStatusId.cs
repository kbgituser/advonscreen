using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class advertisementStatusId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdvertisementStatusId",
                table: "Advertisements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_AdvertisementStatusId",
                table: "Advertisements",
                column: "AdvertisementStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisements_AdvertisementStatuses_AdvertisementStatusId",
                table: "Advertisements",
                column: "AdvertisementStatusId",
                principalTable: "AdvertisementStatuses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisements_AdvertisementStatuses_AdvertisementStatusId",
                table: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_AdvertisementStatusId",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "AdvertisementStatusId",
                table: "Advertisements");
        }
    }
}
