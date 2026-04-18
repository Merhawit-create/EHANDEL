
using Ehandel;
using Ehandel.Methods;
using Ehandel.Models;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "shop.db"));

        await using var db = new ShopContext();

        // Apply migrations
        await db.Database.MigrateAsync();

        // Seed customers
        if (!await db.Customers.AnyAsync())
        {
            db.Customers.AddRange(
                new Customer { Name = "Merhawit", Email = "merhawitristet431@gmail.com", City = "Stockholm" },
               new Customer { Name = "Anna", Email = "AnnaErik431@gmail.com", City = "Keren" }
            );
            await db.SaveChangesAsync();
            Console.WriteLine("Customer Seeded DB");
        }
        
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "Fruits" },   
                new Category { Name = "Drinks" }
            );

            await db.SaveChangesAsync();
            Console.WriteLine("Categories seeded DB");
        }


        // Seed products
        if (!await db.Products.AnyAsync())
        {
            // NYTT: hämta CategoryId för Fruits och Drinks från databasen
            var fruitsId = await db.Categories
                .Where(c => c.Name == "Fruits")
                .Select(c => c.CategoryId)
                .FirstAsync();

            var drinksId = await db.Categories
                .Where(c => c.Name == "Drinks")
                .Select(c => c.CategoryId)
                .FirstAsync();

            db.Products.AddRange(
                new Product { ProductPrice = 10, ProductName = "Apple",CategoryId = fruitsId  },
                new Product { ProductPrice = 20, ProductName = "Orange",CategoryId = fruitsId  },
                new Product { ProductPrice = 30, ProductName = "Banana" ,CategoryId = fruitsId },
                new Product { ProductPrice = 40, ProductName = "Milk",CategoryId = drinksId },
                new Product { ProductPrice = 50, ProductName = "Musli" ,CategoryId = drinksId},
                new Product { ProductPrice = 60, ProductName = "Water" ,CategoryId = drinksId}
            );
            await db.SaveChangesAsync();
            Console.WriteLine("Products Seeded DB");
        }

        // Huvudmeny
        while (true)
        {
            Console.WriteLine("1 = Customers | 2 Orders | 3 Product | 4 Category | 5 = exit");
            var huvudmeny = Console.ReadLine() ?? string.Empty;

            switch (huvudmeny)
            {
                case "1":
                    await CustomerMenuAsync();
                    break;

                case "2":
                    await OrderMenuAsync();
                    break;

                case "3":
                    await ProductMenuAsync();
                    break;
                case "4":
                    await CategoryMenuAsync();
                    break;

                case "5":
                    Console.WriteLine("Exiting...");
                    return;

                default:
                    Console.WriteLine("Please enter a valid option");
                    break;
            }
        }
    }

    // -------- Customer Menu --------
    private static async Task CustomerMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("1 = ListCustomers | 2 AddCustomer | 3 EditCustomer | 4 DeleteCustomer |  5 FilterByCity | 6 = return");
            var customerMenu = Console.ReadLine() ?? string.Empty;

            switch (customerMenu)
            {
                case "1":
                    await CustomerMethod.ListCustomersAsync();
                    break;

                case "2":
                    await CustomerMethod.AddCustomerAsync();
                    break;

                case "3":
                    Console.Write("CustomerId: ");
                    if (int.TryParse(Console.ReadLine(), out var editId))
                        await CustomerMethod.EditCustomerAsync(editId);
                    else
                        Console.WriteLine("Invalid id");
                    break;

                case "4":
                    Console.Write("CustomerId: ");
                   if (int.TryParse(Console.ReadLine(), out var deleteId))
                        await CustomerMethod.DeleteCustomerAsync(deleteId);
                    else
                        Console.WriteLine("Invalid id");
                    break;
                case "5":
                    await CustomerMethod.FilterByCityAsync();
                    break;

                case "6":
                    Console.WriteLine("Returning...");
                    return;

                default:
                    Console.WriteLine("Please enter a valid option");
                    break;
            }
        }
    }

    // -------- Product Menu --------
    private static async Task ProductMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("1 = ListProduct | 2 AddProduct | 3 = EditProduct | 4 = DeleteProduct | 5 = return");
            var productMenu = Console.ReadLine() ?? string.Empty;

            switch (productMenu)
            {
                case "1":
                    await ProductMethod.ListProductsAsync();
                    break;
                case "2":
                    await ProductMethod.AddProductAsync();
                    break;
                case "3":
                   // await ProductMethod.EditProductAsync();
                   Console.Write("ProductId: ");
                   if (int.TryParse(Console.ReadLine(), out var editId))
                       await ProductMethod.EditProductAsync(editId);
                   else
                       Console.WriteLine("Invalid id");
                    break;
                case "4":
                    Console.Write("ProductId: ");
                    if (int.TryParse(Console.ReadLine(), out var deleteId))
                        await ProductMethod.DeleteProductAsync(deleteId);
                    else
                        Console.WriteLine("Invalid id");
                    break;

                case "5":
                    Console.WriteLine("Returning...");
                    return;

                default:
                    Console.WriteLine("Please enter a valid option");
                    break;
            }
        }
    }

    // -------- Order Menu --------
    private static async Task OrderMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("1 = ListOrders | 2 AddOrder | 5 = return");
            var orderMenu = Console.ReadLine() ?? string.Empty;

            switch (orderMenu)
            {
                case "1":
                    Console.Write("Page: ");
                    if (!int.TryParse(Console.ReadLine(), out var page))
                        page = 1;

                    Console.Write("PageSize: ");
                    if (!int.TryParse(Console.ReadLine(), out var pageSize))
                        pageSize = 10;

                    await OrderMethod.ListOrdersPagedAsync(page, pageSize);
                    break;

                case "2":
                    await OrderMethod.AddOrderAsync();
                    break;

                case "5":
                    Console.WriteLine("Returning...");
                    return;

                default:
                    Console.WriteLine("Please enter a valid option");
                    break;
            }
        }
    }
    // NYTT: Meny för full CRUD på kategorier
    private static async Task CategoryMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("1 = ListCategories | 2 = AddCategory | 3 = EditCategory | 4 = DeleteCategory | 5 = Return");
            var categoryMenu = Console.ReadLine() ?? string.Empty;

            switch (categoryMenu)
            {
                case "1":
                    await CategoryMethod.ListCategoriesAsync();
                    break;
                case "2":
                    await CategoryMethod.AddCategoryAsync();
                    break;
                case "3":
                    Console.Write("CategoryId: ");
                    if (int.TryParse(Console.ReadLine(), out var editId))
                        await CategoryMethod.EditCategoryAsync(editId);
                    else
                        Console.WriteLine("Invalid id");
                    break;
                case "4":
                    Console.Write("CategoryId: ");
                    if (int.TryParse(Console.ReadLine(), out var deleteId))
                        await CategoryMethod.DeleteCategoryAsync(deleteId);
                    else
                        Console.WriteLine("Invalid id");
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Please enter a valid option");
                    break;
            }
        }
    }
}
