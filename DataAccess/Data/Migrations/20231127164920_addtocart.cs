using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Data.Migrations
{
    public partial class addtocart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "CartDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "CartCustomozationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvailableCustomozationId = table.Column<int>(type: "int", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartCustomozationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartCustomozationDetails_AvailableCustomozations_AvailableCustomozationId",
                        column: x => x.AvailableCustomozationId,
                        principalTable: "AvailableCustomozations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartCustomozationDetails_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartCustomozationDetails_AvailableCustomozationId",
                table: "CartCustomozationDetails",
                column: "AvailableCustomozationId");

            migrationBuilder.CreateIndex(
                name: "IX_CartCustomozationDetails_CartId",
                table: "CartCustomozationDetails",
                column: "CartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartCustomozationDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "CartDetails");
        }
    }
}
