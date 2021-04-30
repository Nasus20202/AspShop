using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopWebApp.Migrations
{
    public partial class productUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LongAbout",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherPhotos",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LongAbout",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OtherPhotos",
                table: "Products");
        }
    }
}
