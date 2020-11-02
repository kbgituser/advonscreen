using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class advertisementPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PointId",
                table: "Advertisements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_PointId",
                table: "Advertisements",
                column: "PointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisements_Points_PointId",
                table: "Advertisements",
                column: "PointId",
                principalTable: "Points",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisements_Points_PointId",
                table: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_PointId",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "PointId",
                table: "Advertisements");
        }
    }
}
