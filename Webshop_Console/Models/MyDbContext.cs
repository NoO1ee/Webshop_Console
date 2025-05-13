using Microsoft.EntityFrameworkCore;

namespace Webshop_Console.Models;

public class MyDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Authority> Authorities { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Unit> Units { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(@"Server=tcp:noo1edb.database.windows.net,1433;Initial Catalog=noo1e;Persist Security Info=False;User ID=noo1e;Password=MrVYGcjJbTLB2$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Supplier)
            .WithMany(s => s.Articles)
            .HasForeignKey(a => a.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Unit)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Discount)
            .WithMany(d => d.Articles)
            .HasForeignKey(a => a.DiscountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Payment)
            .WithOne(o => o.Order)
            .HasForeignKey<Payment>(p => p.OrderId);

        modelBuilder.Entity<Authority>().HasData(
            new Authority { Id = 1, Name = "User", IsAdmin = false, IsOwner = false },
            new Authority { Id = 2, Name = "Admin", IsAdmin = true, IsOwner = false },
            new Authority { Id = 3, Name = "Owner", IsAdmin = true, IsOwner = true }
        );

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "SpyVer"},
            new Supplier { Id = 2, Name = "Assultis"}
        );

        modelBuilder.Entity<Unit>().HasData(
            new Unit { Id = 1, Name = "Styck", Symbol = "St"},
            new Unit { Id = 2, Name = "Kilo", Symbol = "Kg"}
        );

        modelBuilder.Entity<Discount>().HasData(
            new Discount { Id = 1, Name = "Ingen rabatt", Percentage = 0},
            new Discount { Id = 2, Name = "Silver medlem", Percentage = 20},
            new Discount { Id = 3, Name = "Guld medlem", Percentage = 50}
        );
    }
}
