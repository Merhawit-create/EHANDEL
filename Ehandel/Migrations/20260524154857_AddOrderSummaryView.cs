using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehandel.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderSummaryView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 CREATE VIEW IF NOT EXISTS OrderSummaryView AS
                                 SELECT
                                     o.OrderId,
                                     o.OrderDate,
                                     o.Status,
                                     o.TotalAmount,
                                     c.Name AS CustomerName,
                                     c.Email AS CustomerEmail
                                 FROM Orders o
                                 JOIN Customers c ON c.CustomerId = o.CustomerId;
                                 """);
            // AFTER INSERT
            migrationBuilder.Sql("""
                                 CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Insert
                                 AFTER INSERT ON OrderRows
                                 BEGIN
                                     UPDATE Orders
                                     SET TotalAmount = (
                                         SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                                         FROM OrderRows
                                         WHERE OrderId = NEW.OrderId
                                     )
                                     WHERE OrderId = NEW.OrderId;
                                 END;
                                 """);
            // AFTER UPDATE
            migrationBuilder.Sql("""
                                 CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Update
                                 AFTER UPDATE ON OrderRows
                                 BEGIN
                                     UPDATE Orders
                                     SET TotalAmount = (
                                         SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                                         FROM OrderRows
                                         WHERE OrderId = NEW.OrderId
                                     )
                                     WHERE OrderId = NEW.OrderId;
                                 END;
                                 """);
            // AFTER DELETE
            migrationBuilder.Sql("""
                                 CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Delete
                                 AFTER DELETE ON OrderRows
                                 BEGIN
                                     UPDATE Orders
                                     SET TotalAmount = (
                                         SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                                         FROM OrderRows
                                         WHERE OrderId = OLD.OrderId
                                     )
                                     WHERE OrderId = OLD.OrderId;
                                 END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_OrderRow_Insert;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_OrderRow_Update;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_OrderRow_Delete;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS OrderSummaryView;");
        }
    }
}
