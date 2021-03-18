using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class userpoints2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPoints_AspNetUsers_UserId",
                table: "UserPoints");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPoints_AspNetUsers_UserId",
                table: "UserPoints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPoints_AspNetUsers_UserId",
                table: "UserPoints");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPoints_AspNetUsers_UserId",
                table: "UserPoints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
