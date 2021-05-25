using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopWebApp.Migrations
{
    public partial class Orderupdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailPhone",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "ClientPhone",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientPhone",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "EmailPhone",
                table: "Orders",
                type: "text",
                nullable: true);
        }
    }
}
