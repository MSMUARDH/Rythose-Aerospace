using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Data.Migrations
{
    public partial class addcityintoshippingdetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "ShippingDetail");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "ShippingDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDetail_CityId",
                table: "ShippingDetail",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingDetail_City_CityId",
                table: "ShippingDetail",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingDetail_City_CityId",
                table: "ShippingDetail");

            migrationBuilder.DropIndex(
                name: "IX_ShippingDetail_CityId",
                table: "ShippingDetail");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "ShippingDetail");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ShippingDetail",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
