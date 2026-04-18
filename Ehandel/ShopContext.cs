using Ehandel.Models;
using Microsoft.EntityFrameworkCore;

namespace Ehandel;

public class ShopContext : DbContext 
{
    //Mappar datan mot tabellerna i databasen,(Översätta C#-klasser till riktiga tabeller i databasen.)
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>(); 
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();


protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var dbPath = Path.Combine(AppContext.BaseDirectory, "shop.db");

    optionsBuilder.UseSqlite($"Filename={dbPath}");
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    
    modelBuilder.Entity<Category>(e =>
    {
        e.HasKey(x => x.CategoryId);
        e.Property(x => x.Name).IsRequired().HasMaxLength(100);
        e.HasIndex(x => x.Name).IsUnique();
        e.HasMany(x => x.Products)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);
    });
    
    modelBuilder.Entity<Customer>(e =>
    {
        // Sätter primär nyckel
        e.HasKey(x => x.CustomerId);

        // Säkerställer samma regler som data annotatuibs ( required + MaxLength)
        e.Property(x => x.Name)
            .IsRequired().HasMaxLength(100);
        e.Property(x => x.Email)
            .IsRequired().HasMaxLength(100);
        e.Property(x => x.City).HasMaxLength(100);


        e.HasIndex(x => x.Email).IsUnique();
        e.HasMany(x => x.Orders); 
    });
    
    modelBuilder.Entity<Product>(e =>
    {
        e.HasKey(x => x.ProductId);

        e.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(100);
        e.Property(x => x.Description)
            .HasMaxLength(100);
        e.Property(x => x.ProductPrice)
            .IsRequired();
    });
    
    modelBuilder.Entity<Order>(e =>
    {
        e.HasKey(x => x.OrderId);
        //properties
        e.Property(x => x.OrderDate).IsRequired();
        e.Property(X => X.Status).IsRequired().HasMaxLength(50);
        e.Property(x => x.TotalAmount).IsRequired();
        //relationen Customer 1- N 
        e.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    
    modelBuilder.Entity<OrderRow>(e =>
    {
        // PK
        e.HasKey(x => x.OrderRowId);

        e.Property(x => x.Quantity).IsRequired();
        e.Property(x => x.UnitPrice).IsRequired();
        // order 1 -N orderRows 
        e.HasOne(x => x.Order)
            .WithMany(x => x.OrderRows)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        // Product 1 - M orderRows
        e.HasOne(x => x.Product)
            .WithMany(x => x.OrderRows)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
    });
}
}