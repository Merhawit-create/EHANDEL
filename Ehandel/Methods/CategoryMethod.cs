using Ehandel.Models;
using Microsoft.EntityFrameworkCore;

namespace Ehandel.Methods;

public class CategoryMethod
{
   
    
    
        public static async Task ListCategoriesAsync()
        {
            using var db = new ShopContext();

            var rows = await db.Categories
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

            Console.WriteLine("Id | Name");
            foreach (var row in rows)
            {
                Console.WriteLine($"{row.CategoryId} | {row.Name}");
            }
        }

        
        public static async Task AddCategoryAsync()
        {
            using var db = new ShopContext();

            Console.Write("Category name: ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            {
                Console.WriteLine("Category name is required and max 100 chars.");
                return;
            }

            var exists = await db.Categories.AnyAsync(x => x.Name == name);
            if (exists)
            {
                Console.WriteLine("Category already exists.");
                return;
            }

            await db.Categories.AddAsync(new Category { Name = name });

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Category added.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DB Error: " + ex.GetBaseException().Message);
            }
        }

        // NYTT: Redigerar kategori.
        public static async Task EditCategoryAsync(int categoryId)
        {
            using var db = new ShopContext();

            var category = await db.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
            if (category == null)
            {
                Console.WriteLine("Category not found.");
                return;
            }

            Console.WriteLine($"Current name: {category.Name}");
            Console.Write("New name: ");
            var newName = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(newName))
            {
                Console.WriteLine("No changes made.");
                return;
            }
            if (newName.Length > 100)
            {
                Console.WriteLine("Category name can be max 100 chars.");
                return;
            }
            var exists = await db.Categories
                .AnyAsync(x => x.Name == newName && x.CategoryId != categoryId);

            if (exists)
            {
                Console.WriteLine("Category name already exists.");
                return;
            }
            category.Name = newName;

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Category updated.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DB Error: " + ex.GetBaseException().Message);
            }
        }

        // NYTT: Tar bort kategori.
        public static async Task DeleteCategoryAsync(int categoryId)
        {
            using var db = new ShopContext();

            var category = await db.Categories
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId);

            if (category == null)
            {
                Console.WriteLine("Category not found.");
                return;
            }

            if (category.Products.Any())
            {
                Console.WriteLine("Cannot delete category because it contains products.");
                return;
            }

            db.Categories.Remove(category);

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Category deleted.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DB Error: " + ex.GetBaseException().Message);
            }
        }
    }

