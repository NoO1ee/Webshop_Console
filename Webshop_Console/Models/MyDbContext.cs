using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

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

        modelBuilder.Entity<ArticleModel>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

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
            new SupplierModel { Id = 2, Name = "Assultis"},
            new SupplierModel { Id = 3, Name = "Defensia" },
            new SupplierModel { Id = 4, Name = "TechnoQuack" },
            new SupplierModel { Id = 5, Name = "MysticMallard" },
            new SupplierModel { Id = 6, Name = "Elemental Forces" },
            new SupplierModel { Id = 7, Name = "Nature's Guardians" },
            new SupplierModel { Id = 8, Name = "Shadow Syndicate" },
            new SupplierModel { Id = 9, Name = "Fighter's Forge" }
        );

        modelBuilder.Entity<UnitModel>().HasData(
            new UnitModel { Id = 1, Name = "Healer", Symbol = "Präst"},
            new UnitModel { Id = 2, Name = "Assassin", Symbol = "Mödare"},
            new UnitModel { Id = 3, Name = "Tank", Symbol = "Sköld" },
            new UnitModel { Id = 4, Name = "Mage", Symbol = "Magiker" },
            new UnitModel { Id = 5, Name = "Warrior", Symbol = "Krigare" },
            new UnitModel { Id = 6, Name = "Ranger", Symbol = "Bågskytt" },
            new UnitModel { Id = 7, Name = "Engineer", Symbol = "Mekaniker" }

        );

        modelBuilder.Entity<Discount>().HasData(
            new Discount { Id = 1, Name = "Ingen rabatt", Percentage = 0},
            new Discount { Id = 2, Name = "Silver medlem", Percentage = 20},
            new Discount { Id = 3, Name = "Guld medlem", Percentage = 50}
        );

        modelBuilder.Entity<CategoryModel>().HasData(
            new CategoryModel { Id = 1, Name = "Legendarisk"},
            new CategoryModel { Id = 2, Name = "Episk"},
            new CategoryModel { Id = 3, Name = "Basic"}
        );

        modelBuilder.Entity<PaymentMethod>().HasData(
        
            new PaymentMethod { Id = 1, Name = "Kortbetalning"},
            new PaymentMethod { Id = 2, Name = "Swish"},
            new PaymentMethod { Id = 3, Name = "Faktura"},
            new PaymentMethod { Id = 4, Name = "PayPal" },
            new PaymentMethod { Id = 5, Name = "Kryptovaluta" },
            new PaymentMethod { Id = 6, Name = "Kontant" },
            new PaymentMethod { Id = 7, Name = "Delbetalning" },
            new PaymentMethod { Id = 8, Name = "Presentkort" },
            new PaymentMethod { Id = 9, Name = "Autogiro" },
            new PaymentMethod { Id = 10, Name = "Adex" }
        );

        modelBuilder.Entity<ArticleModel>().HasData(
    new ArticleModel
    {
        Id = 1,
        Name = "Quenton Quack",
        EanCode = "30291823",
        ArticleCode = "344233",
        Storage = 200,
        Price = 3000m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Skuggornas mördare - osynlig och snabb",
        CategoryId = 3
    },
    new ArticleModel
    {
        Id = 2,
        Name = "Beatrix Drake",
        EanCode = "20230304",
        ArticleCode = "430233",
        Storage = 2,
        Price = 50000m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Mästarens prickskytt - ser från mils avstånd",
        CategoryId = 1 
    },
    new ArticleModel
    {
        Id = 3,
        Name = "Agent Mallory Mallard",
        EanCode = "23232123",
        ArticleCode = "414663",
        Storage = 42,
        Price = 4499m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = true,
        Bio = "Spion-anka - infiltrerar fiendelinjer",
        CategoryId = 3 
    },
    new ArticleModel
    {
        Id = 4,
        Name = "Archibald Quill",
        EanCode = "12312312",
        ArticleCode = "748454",
        Storage = 32,
        Price = 15000m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Skogens väktare - bågskytt och överlevare",
        CategoryId = 2 
    },
    new ArticleModel
    {
        Id = 5,
        Name = "Magnus Mallard",
        EanCode = "44512315",
        ArticleCode = "112577",
        Storage = 15,
        Price = 12999m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = true,
        Bio = "Elementarkrigare - kontrollerar eld och is",
        CategoryId = 2
    },
    new ArticleModel
    {
        Id = 6,
        Name = "Helga Mallard",
        EanCode = "04912454",
        ArticleCode = "245552",
        Storage = 16,
        Price = 5600m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Helande räddare - botar och stärker allierade",
        CategoryId = 3
    },
    new ArticleModel
    {
        Id = 7,
        Name = "Bruno Beak",
        EanCode = "07669964",
        ArticleCode = "692042",
        Storage = 6,
        Price = 4420m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Oövervinnerlig - drar åt sig all fiendeuppmärksamhet",
        CategoryId = 3
    },
    new ArticleModel
    {
        Id = 8,
        Name = "Greta Quack",
        EanCode = "50042244",
        ArticleCode = "024132",
        Storage = 4,
        Price = 11000m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Mekaniker-anka - bygger fällor och turrets",
        CategoryId = 2
    },
    new ArticleModel
    {
        Id = 9,
        Name = "Finn Feather",
        EanCode = "12499902",
        ArticleCode = "400234",
        Storage = 1,
        Price = 90300m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Ismagikern - fryser fiender på plats",
        CategoryId = 1
    },
    new ArticleModel
    {
        Id = 10,
        Name = "Roland Drake",
        EanCode = "29490501",
        ArticleCode = "932491",
        Storage = 3,
        Price = 159000m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Vildsint kämpe - kullkastar allt som står i vägen",
        CategoryId = 1
    },
    new ArticleModel
    {
        Id = 11,
        Name = "Lyra Quill",
        EanCode = "91204329",
        ArticleCode = "233993",
        Storage = 1,
        Price = 1500m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Helig präst - välsignar och fördriver mörker",
        CategoryId = 3
    },
    new ArticleModel
    {
        Id = 12,
        Name = "Shadow Mallard",
        EanCode = "49402123",
        ArticleCode = "499532",
        Storage = 3,
        Price = 4030m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Listig tjuv - snor skatter och smyger förbi vakter",
        CategoryId = 3
    },
    new ArticleModel
    {
        Id = 13,
        Name = "Fighter Ducker",
        EanCode = "590239583",
        ArticleCode = "230032",
        Storage = 2,
        Price = 4223m,
        SupplierId = 1,
        UnitId = 1,
        DiscountId = 1,
        IsFeatured = false,
        Bio = "Test Anka",
        CategoryId = 2 
    }
);
    }
}
