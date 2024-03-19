using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Data.Migrations
{
    public partial class paymenttbl2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Payment_PaymentID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentID",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrdersID",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrdersID",
                table: "Payment",
                column: "OrdersID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Orders_OrdersID",
                table: "Payment",
                column: "OrdersID",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Orders_OrdersID",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_OrdersID",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "OrdersID",
                table: "Payment");

            migrationBuilder.AddColumn<int>(
                name: "PaymentID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentID",
                table: "Orders",
                column: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Payment_PaymentID",
                table: "Orders",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
