using Ehandel.Helpers;
using Ehandel.Models;
using Microsoft.EntityFrameworkCore;

namespace Ehandel.Methods;

public class OrderMethod
{
     public static async Task ListOrdersPagedAsync(int page, int pageSize)
    {
        using var db = new ShopContext();
        // Start timer for performance measurement
        var sw = System.Diagnostics.Stopwatch.StartNew();
        // Build query
        var query = db.Orders
            .Include(x => x.Customer)
            .AsNoTracking()
            .OrderByDescending(x => x.OrderDate)
            .ThenByDescending(x => x.TotalAmount); 
         // Count total orders
        var totalCount = await query.CountAsync();
        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        // Get only orders for current page
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        // Stop timer
        sw.Stop();
        // Show query time
        Console.WriteLine($"Query time: {sw.ElapsedMilliseconds} ms");
        if (totalPages == 0)
        {
            Console.WriteLine("No orders found.");
            return;
        }
        Console.WriteLine($"Page {page} / {totalPages}, pageSize = {pageSize}");
        // Print orders
        foreach (var order in orders)
        {       
            
            // Decrypt customer email before showing it
            var decryptedEmail = order.Customer != null
                ? EncryptionHelper.Decrypt(order.Customer.Email)
                : "No Email";

            Console.WriteLine(
                $"{order.OrderId} | {order.OrderDate} | {order.TotalAmount:c} | {decryptedEmail}");
        }
    }

    // AddOrderAsync - Metod för att lägga till en order med orderrader. Använd transaktioner för att säkerställa att antingen hela ordern läggs till eller ingen alls, särskilt när du uppdaterar lagersaldot.
    public static async Task AddOrderAsync()
    {
        using var db = new ShopContext();
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
        var customers = await db.Customers.AsNoTracking()
            .OrderBy(x => x.CustomerId)
            .ToListAsync();

        if (!customers.Any())
        {
            Console.WriteLine("No Customer Found");
            return;
        }

        foreach (var customer in customers)
        {
            Console.WriteLine($"{customer.CustomerId} | {customer.Name} | {customer.Email}");
        }

        Console.Write("CustomerId: ");
        if (!int.TryParse(Console.ReadLine(), out var customerId) ||
            !customers.Any(x => x.CustomerId == customerId))
        {
            Console.WriteLine("Invalid input of customerId");
            return;
        }

        var order = new Order
        {
            CustomerId = customerId,
            OrderDate = DateTime.Now,
            Status = "Pending",
            TotalAmount = 0
           
        };
        var OrderRows = new List<OrderRow>();
        while (true)
        {
            Console.WriteLine("Add Order row? Y/N");
            var answer = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (answer != "y") break;

            var products = await db.Products.AsNoTracking()
                .OrderBy(x => x.ProductId)
                .ToListAsync();

            if (!products.Any())
            {
                Console.WriteLine("No Product Found");
                return;
            }
            Console.WriteLine("ProductId | Name | Price | Stock");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductId} | {product.ProductName} | {product.ProductPrice}| Stock: {product.StockQuantity}");
            }

            Console.Write("ProductId: ");
            if (!int.TryParse(Console.ReadLine(), out var productId))
            {
                Console.WriteLine("Invalid input of productId");
                continue;
            }

            var chosenProduct = await db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (chosenProduct == null)
            {
                Console.WriteLine("Product not found");
                continue;
            }

            Console.Write("Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid input of quantity");
                continue;
            }
            // Om lagret är för lågt kastas ett fel och transaktionen rollbackas.
            if (chosenProduct.StockQuantity < quantity)
            {
                throw new Exception($"Not enough stock for {chosenProduct.ProductName}. Available: {chosenProduct.StockQuantity}");
            }
            var row = new OrderRow
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = chosenProduct.ProductPrice
            };

            OrderRows.Add(row);
            //  Uppdaterar lagersaldo.
            chosenProduct.StockQuantity -= quantity;
        }
        //  Stoppar order om inga orderrader har lagts till.
        if (!OrderRows.Any())
        {
            Console.WriteLine("Order cancelled. No order rows added.");
            await transaction.RollbackAsync();
            return;
        }

        order.OrderRows =OrderRows;
        order.TotalAmount = OrderRows.Sum(x => x.UnitPrice*x.Quantity);
        db.Orders.Add(order);
        
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        Console.WriteLine($"Order {order.OrderId} created, Total = {order.TotalAmount:c} creat!");
        }
        catch (DbUpdateException exception)
        {
            // Om något går fel ångras hela ordern.
            await transaction.RollbackAsync();
            Console.WriteLine("Order cancelled. Rollback executed.");
            Console.WriteLine("DB Error : " + exception.GetBaseException().Message);
        }
    }
    
    public static async Task ListOrderSummaryViewAsync()
    {
        using var db = new ShopContext();

        var orders = await db.OrderSummaryViews
            .AsNoTracking()
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();

        Console.WriteLine("OrderId | Date | Status | Total | Customer | Email");

        foreach (var order in orders)
        {
            var email = EncryptionHelper.Decrypt(order.CustomerEmail);

            Console.WriteLine(
                $"{order.OrderId} | {order.OrderDate} | {order.Status} | {order.TotalAmount:c} | {order.CustomerName} | {email}");
        }
    }
}