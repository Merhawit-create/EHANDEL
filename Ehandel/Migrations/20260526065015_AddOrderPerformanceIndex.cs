using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehandel.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPerformanceIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate_OrderId",
                table: "Orders",
                columns: new[] { "OrderDate", "OrderId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderDate_OrderId",
                table: "Orders");
        }
    }
}
