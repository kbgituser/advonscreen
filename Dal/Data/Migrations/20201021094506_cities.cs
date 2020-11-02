using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dal.Data.Migrations
{
    public partial class cities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Advertisements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AdvertisementStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvId = table.Column<int>(nullable: false),
                    AdvertisementId = table.Column<int>(nullable: true),
                    AdvertisementStatusId = table.Column<int>(nullable: false),
                    ChangeDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementStatusHistories_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdvertisementStatusHistories_AdvertisementStatuses_AdvertisementStatusId",
                        column: x => x.AdvertisementStatusId,
                        principalTable: "AdvertisementStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementStatusHistories_AdvertisementId",
                table: "AdvertisementStatusHistories",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementStatusHistories_AdvertisementStatusId",
                table: "AdvertisementStatusHistories",
                column: "AdvertisementStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementStatusHistories");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "AdvertisementStatuses");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Advertisements");
        }
    }
}
