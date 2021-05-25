using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopWebApp.Migrations
{
    public partial class shipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingInfo",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingType",
                table: "Orders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingInfo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingType",
                table: "Orders");
        }
    }
}
