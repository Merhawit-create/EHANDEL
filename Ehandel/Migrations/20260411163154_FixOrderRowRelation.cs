using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehandel.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderRowRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRows_Products_ProductId1",
                table: "OrderRows");

            migrationBuilder.DropIndex(
                name: "IX_OrderRows_ProductId1",
                table: "OrderRows");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "OrderRows");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "OrderRows",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderRows_ProductId1",
                table: "OrderRows",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRows_Products_ProductId1",
                table: "OrderRows",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "ProductId");
        }
    }
}
