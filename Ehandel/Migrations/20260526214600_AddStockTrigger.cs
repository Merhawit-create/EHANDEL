using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehandel.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                     CREATE TRIGGER IF NOT EXISTS PreventNegativeStock
                                     BEFORE INSERT ON OrderRows
                                     FOR EACH ROW
                                     WHEN (
                                         SELECT StockQuantity
                                         FROM Products
                                         WHERE ProductId = NEW.ProductId
                                     ) < NEW.Quantity
                                     BEGIN
                                         SELECT RAISE(ABORT, 'Not enough stock');
                                     END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                     DROP TRIGGER IF EXISTS PreventNegativeStock;
                                 """);

        }
    }
}
