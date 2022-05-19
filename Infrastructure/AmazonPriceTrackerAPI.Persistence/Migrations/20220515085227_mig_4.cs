using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazonPriceTrackerAPI.Persistence.Migrations
{
    public partial class mig_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PriceChange",
                table: "TrackedProducts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceChange",
                table: "TrackedProducts");
        }
    }
}
