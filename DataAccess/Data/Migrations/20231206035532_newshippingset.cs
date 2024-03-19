using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Data.Migrations
{
    public partial class newshippingset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingSet_City_Id",
                table: "ShippingSet");

            migrationBuilder.DropIndex(
                name: "IX_ShippingSet_Id",
                table: "ShippingSet");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ShippingSet");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "ShippingSet",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingSet_CityId",
                table: "ShippingSet",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingSet_City_CityId",
                table: "ShippingSet",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingSet_City_CityId",
                table: "ShippingSet");

            migrationBuilder.DropIndex(
                name: "IX_ShippingSet_CityId",
                table: "ShippingSet");

            migrationBuilder.AlterColumn<string>(
                name: "CityId",
                table: "ShippingSet",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ShippingSet",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingSet_Id",
                table: "ShippingSet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingSet_City_Id",
                table: "ShippingSet",
                column: "Id",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
