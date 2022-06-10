using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazonPriceTrackerAPI.Persistence.Migrations
{
    public partial class mig_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isTracking",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isTracking",
                table: "Products");
        }
    }
}
