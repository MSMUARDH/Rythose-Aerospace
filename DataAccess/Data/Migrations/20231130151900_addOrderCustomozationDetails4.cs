using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Data.Migrations
{
    public partial class addOrderCustomozationDetails4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartCustomozationDetails_AvailableCustomozations_AvailableCustomozationId",
                table: "CartCustomozationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartCustomozationDetails_Carts_CartId",
                table: "CartCustomozationDetails");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "CartCustomozationDetails",
                newName: "CartDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_CartCustomozationDetails_CartId",
                table: "CartCustomozationDetails",
                newName: "IX_CartCustomozationDetails_CartDetailsId");

            migrationBuilder.AlterColumn<int>(
                name: "AvailableCustomozationId",
                table: "CartCustomozationDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "OrderCustomozationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvailableCustomozationId = table.Column<int>(type: "int", nullable: true),
                    OrderDetailsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomozationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCustomozationDetails_AvailableCustomozations_AvailableCustomozationId",
                        column: x => x.AvailableCustomozationId,
                        principalTable: "AvailableCustomozations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderCustomozationDetails_OrderDetails_OrderDetailsId",
                        column: x => x.OrderDetailsId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomozationDetails_AvailableCustomozationId",
                table: "OrderCustomozationDetails",
                column: "AvailableCustomozationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomozationDetails_OrderDetailsId",
                table: "OrderCustomozationDetails",
                column: "OrderDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartCustomozationDetails_AvailableCustomozations_AvailableCustomozationId",
                table: "CartCustomozationDetails",
                column: "AvailableCustomozationId",
                principalTable: "AvailableCustomozations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCustomozationDetails_CartDetails_CartDetailsId",
                table: "CartCustomozationDetails",
                column: "CartDetailsId",
                principalTable: "CartDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartCustomozationDetails_AvailableCustomozations_AvailableCustomozationId",
                table: "CartCustomozationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartCustomozationDetails_CartDetails_CartDetailsId",
                table: "CartCustomozationDetails");

            migrationBuilder.DropTable(
                name: "OrderCustomozationDetails");

            migrationBuilder.RenameColumn(
                name: "CartDetailsId",
                table: "CartCustomozationDetails",
                newName: "CartId");

            migrationBuilder.RenameIndex(
                name: "IX_CartCustomozationDetails_CartDetailsId",
                table: "CartCustomozationDetails",
                newName: "IX_CartCustomozationDetails_CartId");

            migrationBuilder.AlterColumn<int>(
                name: "AvailableCustomozationId",
                table: "CartCustomozationDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCustomozationDetails_AvailableCustomozations_AvailableCustomozationId",
                table: "CartCustomozationDetails",
                column: "AvailableCustomozationId",
                principalTable: "AvailableCustomozations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCustomozationDetails_Carts_CartId",
                table: "CartCustomozationDetails",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
