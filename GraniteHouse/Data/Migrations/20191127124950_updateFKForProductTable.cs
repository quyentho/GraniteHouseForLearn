using Microsoft.EntityFrameworkCore.Migrations;

namespace GraniteHouse.Data.Migrations
{
    public partial class updateFKForProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpecialTagsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpecialTagsId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecialTagID",
                table: "Products",
                column: "SpecialTagID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagID",
                table: "Products",
                column: "SpecialTagID",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpecialTagID",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "SpecialTagsId",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecialTagsId",
                table: "Products",
                column: "SpecialTagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagsId",
                table: "Products",
                column: "SpecialTagsId",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
