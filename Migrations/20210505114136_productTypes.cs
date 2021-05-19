using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopWebApp.Migrations
{
    public partial class productTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Types",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Types",
                table: "Products");
        }
    }
}
