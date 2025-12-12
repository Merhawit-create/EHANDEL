using Microsoft.EntityFrameworkCore;

namespace Ehandel.Methods;

public class ProductMethod
{
    public static async Task ListProductsAsync()
    {
        using var db = new ShopContext();

        var rows = await db.Products
            .AsNoTracking()
            .OrderBy(p => p.ProductName)
            .ToListAsync();

        Console.WriteLine("Id | Name | Price ");
        foreach (var row in rows)
        {
            Console.WriteLine($"{row.ProductId} | {row.ProductName} | {row.ProductPrice}");
        }
    }

    // Här kan du senare lägga till Add/Edit/DeleteProduct om läraren vill
}