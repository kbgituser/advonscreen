using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class advertisementStatusHistory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementStatusHistories_Advertisements_AdvertisementId",
                table: "AdvertisementStatusHistories");

            migrationBuilder.DropColumn(
                name: "AdvId",
                table: "AdvertisementStatusHistories");

            migrationBuilder.AlterColumn<int>(
                name: "AdvertisementId",
                table: "AdvertisementStatusHistories",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementStatusHistories_Advertisements_AdvertisementId",
                table: "AdvertisementStatusHistories",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementStatusHistories_Advertisements_AdvertisementId",
                table: "AdvertisementStatusHistories");

            migrationBuilder.AlterColumn<int>(
                name: "AdvertisementId",
                table: "AdvertisementStatusHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AdvId",
                table: "AdvertisementStatusHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementStatusHistories_Advertisements_AdvertisementId",
                table: "AdvertisementStatusHistories",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
