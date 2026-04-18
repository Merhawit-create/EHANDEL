using Ehandel.Models;
using Microsoft.EntityFrameworkCore;

namespace Ehandel.Methods;

public class ProductMethod
{
    public static async Task ListProductsAsync()
    {
        using var db = new ShopContext();

        var rows = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
        
        
        Console.WriteLine("Id | Name | Price | Category | Description");
        foreach (var row in rows)
        {
            Console.WriteLine($"{row.ProductId} | {row.ProductName} | {row.ProductPrice} | {row.Category?.Name} | {row.Description}");
        }
    }

   
    
    // NYTT: Lägg till produkt.
    public static async Task AddProductAsync()
    {
        using var db = new ShopContext();

        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        if (!categories.Any())
        {
            Console.WriteLine("No categories found. Add a category first.");
            return;
        }

        Console.WriteLine("Available categories:");
        foreach (var category in categories)
        {
            Console.WriteLine($"{category.CategoryId} | {category.Name}");
        }

        Console.Write("Product name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
        {
            Console.WriteLine("Product name is required and max 100 chars.");
            return;
        }

        Console.Write("Price: ");
        if (!decimal.TryParse(Console.ReadLine(), out var price) || price < 0)
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        Console.Write("CategoryId: ");
        if (!int.TryParse(Console.ReadLine(), out var categoryId) ||
            !categories.Any(x => x.CategoryId == categoryId))
        {
            Console.WriteLine("Invalid category id.");
            return;
        }

        Console.Write("Description: ");
        var description = Console.ReadLine()?.Trim();

        await db.Products.AddAsync(new Product
        {
            ProductName = name,
            ProductPrice = price,
            CategoryId = categoryId,
            Description = description
        });

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product added.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("DB Error: " + ex.GetBaseException().Message);
        }
    }

    // NYTT: Redigera produkt.
    public static async Task EditProductAsync(int productId)
    {
        using var db = new ShopContext();

        var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        Console.WriteLine($"Current name: {product.ProductName}");
        Console.Write("New name (leave empty to keep): ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(name))
        {
            product.ProductName = name;
        }

        Console.WriteLine($"Current price: {product.ProductPrice}");
        Console.Write("New price (leave empty to keep): ");
        var priceInput = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(priceInput))
        {
            if (!decimal.TryParse(priceInput, out var price) || price < 0)
            {
                Console.WriteLine("Invalid price.");
                return;
            }
            product.ProductPrice = price;
        }

        Console.WriteLine($"Current description: {product.Description}");
        Console.Write("New description (leave empty to keep): ");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(description))
        {
            product.Description = description;
        }

        Console.WriteLine($"Current categoryId: {product.CategoryId}");
        Console.WriteLine("Available categories:");
        foreach (var category in categories)
        {
            Console.WriteLine($"{category.CategoryId} | {category.Name}");
        }

        Console.Write("New categoryId (leave empty to keep): ");
        var categoryInput = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(categoryInput))
        {
            if (!int.TryParse(categoryInput, out var categoryId) ||
                !categories.Any(x => x.CategoryId == categoryId))
            {
                Console.WriteLine("Invalid categoryId.");
                return;
            }

            product.CategoryId = categoryId;
        }

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product updated.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("DB Error: " + ex.GetBaseException().Message);
        }
    }

    // NYTT: Ta bort produkt.
    public static async Task DeleteProductAsync(int productId)
    {
        using var db = new ShopContext();

        var product = await db.Products
            .Include(x => x.OrderRows)
            .FirstOrDefaultAsync(x => x.ProductId == productId);

        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        if (product.OrderRows.Any())
        {
            Console.WriteLine("Cannot delete product because it is used in orders.");
            return;
        }

        db.Products.Remove(product);

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product deleted.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("DB Error: " + ex.GetBaseException().Message);
        }
    }

}