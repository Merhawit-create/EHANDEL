using Ehandel.Models;
using Microsoft.EntityFrameworkCore;

namespace Ehandel.Methods;

public class CustomerMethod
{
    public static async Task ListCustomersAsync()
    {
     using var db = new ShopContext();

        var customers = await db.Customers
            .AsNoTracking()
            .Include(c => c.Orders)
            //.OrderBy(x => x.CustomerId)
            .OrderByDescending(x => x.CustomerId)
            .ToListAsync();

        Console.WriteLine("Id |   Name   |   City    | Email");
        foreach (var customer in customers)
        {
            Console.WriteLine($"{customer.CustomerId} |  {customer.Name} |   {customer.City}    | {customer.Email}");

            if (customer.Orders != null && customer.Orders.Count > 0)
            {
                Console.WriteLine("  ---- Orders ----");
                foreach (var order in customer.Orders)
                {
                    Console.WriteLine($"    OrderId: {order.OrderId} | TotalAmount: {order.TotalAmount:c}");
                }
            }
        }
    }

    
    
    
    // NYTT: Hämta kunder per stad (som din GetByCity, fast async och med EF-kontekst inuti)
    public static async Task<List<Customer>> GetByCityAsync(string city)
    {
        using var db = new ShopContext();

        // Trimma och säkra att city inte är null
        city = city?.Trim() ?? string.Empty;

        // Om tom sträng – returnera tom lista
        if (string.IsNullOrWhiteSpace(city))
        {
            return new List<Customer>();
        }

        return await db.Customers
            .AsNoTracking()
            .Include(c => c.Orders)              // Samma stil som ListCustomers
            .Where(c => c.City == city)          // Filtrering på stad
            .OrderByDescending(c => c.CustomerId) // Nyaste först, samma som ListCustomers
            .ToListAsync();
    }

    // NYTT: FilterByCity som frågar användaren och skriver ut resultat
    public static async Task FilterByCityAsync()
    {
        Console.Write("Enter city: ");
        var city = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(city))
        {
            Console.WriteLine("City is required.");
            return;
        }

        var customers = await GetByCityAsync(city);

        if (!customers.Any())
        {
            Console.WriteLine($"No customers found in {city}.");
            return;
        }

        Console.WriteLine($"Customers in {city}:");
        Console.WriteLine("Id |   Name   |   City    | Email");

        foreach (var c in customers)
        {
            Console.WriteLine($"{c.CustomerId} |  {c.Name} | {c.City} | {c.Email}");

            // NYTT: Om du vill visa ordrar här också (som i ListCustomers)
            if (c.Orders != null && c.Orders.Count > 0)
            {
                Console.WriteLine("  ---- Orders ----");
                foreach (var order in c.Orders)
                {
                    Console.WriteLine($"    OrderId: {order.OrderId} | TotalAmount: {order.TotalAmount:c}");
                }
            }
        }
    }

    // Add Customer
    public static async Task AddCustomerAsync()
    {
        using var db = new ShopContext();

        Console.Write("Enter Customer Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(name) || name.Length > 100)
        {
            Console.WriteLine("Name is required.");
            return;
        }

        Console.Write("Enter Customer Email: ");
        var email = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrEmpty(email) || email.Length > 250)
        {
            Console.WriteLine("Email is required.");
            return;
        }

        Console.Write("City: ");
        var city = Console.ReadLine() ?? string.Empty;
        if (city.Length > 250)
        {
            Console.WriteLine("City name can't be longer than 250 characters.");
            return;
        }

        await db.Customers.AddAsync(new Customer
        {
            Name = name,
            Email = email,
            City = city
        });

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Customer added.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Db Error! " + ex.GetBaseException().Message);
        }
    }

    // Edit customer <id>
    public static async Task EditCustomerAsync(int customerId)
    {
        using var db = new ShopContext();

        var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        if (customer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        Console.WriteLine("Current Customer Name: " + customer.Name);
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(name))
        {
            customer.Name = name;
        }

        Console.WriteLine("Current Customer Email: " + customer.Email);
        var email = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(email))
        {
            customer.Email = email;
        }

        Console.WriteLine("Current Customer City: " + customer.City);
        var city = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(city))
        {
            customer.City = city;
        }

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Customer edited");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    // Deletecustomer <id>/
    
    public static async Task DeleteCustomerAsync(int customerId)
    {
        using var db = new ShopContext();

        var customer = await db.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        if (customer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }
      
        db.Customers.Remove(customer);

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Customer deleted.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}