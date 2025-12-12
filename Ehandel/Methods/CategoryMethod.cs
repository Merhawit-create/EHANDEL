using Microsoft.EntityFrameworkCore;

namespace Ehandel.Methods;

public class CategoryMethod
{
    public static async Task ListProductsAsync()
    {
        using var db = new ShopContext();

        var rows = await db.Categories
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        Console.WriteLine("Id | Name |  ");
        foreach (var row in rows)
        {
            Console.WriteLine($"{row.CategoryId} | {row.Name} | ");
        }
    }
}