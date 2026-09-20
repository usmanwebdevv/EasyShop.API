using EasyShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyShop.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Order> Orders { get; set; }

public DbSet<OrderItem> OrderItems { get; set; }
public DbSet<Cart> Carts { get; set; }

public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>()
    .HasIndex(c => c.UserId)
    .IsUnique();

modelBuilder.Entity<CartItem>()
    .HasIndex(ci => new { ci.CartId, ci.ProductId })
    .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

modelBuilder.Entity<Order>()
    .Property(o => o.TotalAmount)
    .HasPrecision(18, 2);

modelBuilder.Entity<OrderItem>()
    .Property(oi => oi.UnitPrice)
    .HasPrecision(18, 2);
    }

    
    
}