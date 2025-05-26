using Microsoft.EntityFrameworkCore;

namespace Webshop_Console.Models;

public class MyDbContext : DbContext
{
    public DbSet<ArticleModel> Articles { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<AuthorityModel> Authorities { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PaymentModel> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<SupplierModel> Suppliers { get; set; }
    public DbSet<UnitModel> Units { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(@"Server=tcp:noo1edb.database.windows.net,1433;Initial Catalog=noo1e;Persist Security Info=False;User ID=noo1e;Password=MrVYGcjJbTLB2$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleModel>()
            .HasOne(a => a.Supplier)
            .WithMany(s => s.Articles)
            .HasForeignKey(a => a.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArticleModel>()
            .HasOne(a => a.Unit)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArticleModel>()
            .HasOne(a => a.Discount)
            .WithMany(d => d.Articles)
            .HasForeignKey(a => a.DiscountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderModel>()
            .HasOne(o => o.Payment)
            .WithOne(o => o.Order)
            .HasForeignKey<PaymentModel>(p => p.OrderId);

        modelBuilder.Entity<AuthorityModel>().HasData(
            new AuthorityModel { Id = 1, Name = "User", IsAdmin = false, IsOwner = false },
            new AuthorityModel { Id = 2, Name = "Admin", IsAdmin = true, IsOwner = false },
            new AuthorityModel { Id = 3, Name = "Owner", IsAdmin = true, IsOwner = true }
        );

        modelBuilder.Entity<SupplierModel>().HasData(
            new SupplierModel { Id = 1, Name = "SpyVer"},
            new SupplierModel { Id = 2, Name = "Assultis"}
        );

        modelBuilder.Entity<UnitModel>().HasData(
            new UnitModel { Id = 1, Name = "Styck", Symbol = "St"},
            new UnitModel { Id = 2, Name = "Kilo", Symbol = "Kg"}
        );

        modelBuilder.Entity<Discount>().HasData(
            new Discount { Id = 1, Name = "Ingen rabatt", Percentage = 0},
            new Discount { Id = 2, Name = "Silver medlem", Percentage = 20},
            new Discount { Id = 3, Name = "Guld medlem", Percentage = 50}
        );
    }
}
