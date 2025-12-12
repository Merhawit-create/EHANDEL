using Ehandel.Models;
using Microsoft.EntityFrameworkCore;

namespace Ehandel.Methods;

public class OrderMethod
{
     public static async Task ListOrdersPagedAsync(int page, int pageSize)
    {
        using var db = new ShopContext();

        var query = db.Orders
            .Include(x => x.Customer)
            .AsNoTracking()
            .OrderByDescending(x => x.OrderDate);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)

            .ToListAsync();
        Console.WriteLine($"Page {page} / {totalPages}, pageSize = {pageSize}");
        foreach (var order in orders)
        {
            Console.WriteLine(
                $"{order.OrderId} | {order.OrderDate} | {order.TotalAmount:c} | {order.Customer?.Email}");
        }
    }

    // Lärarens AddOrderAsync flyttad hit och fixad1
    public static async Task AddOrderAsync()
    {
        using var db = new ShopContext();

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

            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductId} | {product.ProductName} | {product.ProductPrice}");
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

            var row = new OrderRow
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = chosenProduct.ProductPrice
            };

            OrderRows.Add(row);
        }

        /*if (order.OrderRows.Count == 0)
        {
            Console.WriteLine("Order cancelled (no rows added).");
            return;
        }*/

     

        order.OrderRows =OrderRows;
        order.TotalAmount = OrderRows.Sum(x => x.UnitPrice*x.Quantity);
        db.Orders.Add(order);

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine($"Order {order.OrderId} created, Total = {order.TotalAmount:c} creat!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine("DB Error : " + exception.GetBaseException().Message);
        }
    }
}