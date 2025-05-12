using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.Models;

internal class MyDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Authoriry> Authorities { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Supplier> Supliers { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=tcp:noo1edb.database.windows.net,1433;Initial Catalog=noo1e;Persist Security Info=False;User ID=noo1e;Password=MrVYGcjJbTLB2$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    }

}
