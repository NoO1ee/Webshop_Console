using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Webshop_Console.Models;
using Webshop_Console.Services;

namespace Webshop_Console.UI;

public class MenuManager
{
    readonly AuthService _auth;
    readonly MyDbContext _db;
    readonly ProductService _productService;
    readonly CartService _cartService;
    readonly PaymentService _paymentService;
    readonly List<Article> _cart = new List<Article>();
    readonly OrderService _orderService;

    User? _currentUser;

    public MenuManager(AuthService auth, MyDbContext db, ProductService productService)
    {
        _auth = auth;
        _db = db;
        _productService = productService;
        _cartService = new CartService(db);
        _paymentService = new PaymentService(db);
        _orderService = new OrderService(db);
    }



    // Kör asynkrona actions i en sync action
    void RunAsync(Func<Task> func) => Task.Run(func).Wait();

    // För att slå ihop label och Func<Task> till string, action
    (string, Action) Option(string label, Func<Task> action) => (label, () => RunAsync(action));

    (string, Action) Option(string label, Action action) => (label, action);

    (string, Action) LogoutOption() => Option("Logga ut", () => ShowMainMenuAsync().Wait());

    async Task HandleLoginAsync()
    {
        _currentUser = await _auth.LoginAsync();
        if (_currentUser != null)
            await RouteByRoleAsync(_currentUser);
    }

    async Task RouteByRoleAsync(User user)
    {
        if (user.Authorities.Any(r => r.IsOwner))
            await ShowOwnerMenuAsync(user);
        else if (user.Authorities.Any(r => r.IsAdmin))
            await ShowAdminMenuAsync();
        else
            await ShowUserMenuAsync();
    }

    #region Menyer

    //Huvud meny
    public async Task ShowMainMenuAsync()
    {
        var options = new[]
        {
            Option("Log in", HandleLoginAsync),
            Option("Register", _auth.RegisterAsync),
            Option("Exit", () => Environment.Exit(0))
        };
        await Menu.ShowMenu("Duck4Hire", "Log in / Register", options: options, titleColor: ConsoleColor.Yellow, textColor: ConsoleColor.White, selectedColor: ConsoleColor.Green, minBoxWidth: 30, boxHeight: 3);
    }


    Task ShowAdminMenuAsync()
    {
        var options = new[]
        {
            Option("Se ordrar", ManageOrderAsync),
            Option("Artiklar", ShowProductManagementMenuAsync),
            LogoutOption()
        };
        return Menu.ShowMenu("Adminpanel", "Adminpanel", options);
    }

    Task ShowOwnerMenuAsync(User owner)
    {
        var options = new []
        {
            Option("Se statistik", ShowStatistics),
            Option("Artiklar", ShowProductManagementMenuAsync),
            Option("Hantera användares roller", ManageUserRolesAsync),
            LogoutOption()
        };

        return Menu.ShowMenu("Ägarpanel", "Ägarpanel", options);
    }

    Task ShowUserMenuAsync()
    {
        var options = new[]
        {
            Option("Visa produkter", ShowProductsAsync),
            Option("Visa kundvagn", ShowCartAsync),
            Option("Se dina ordrar", ShowMyOrdersAsync),
            LogoutOption()
        };
        Console.WriteLine("Test");
        return Menu.ShowMenu("Användarpanel", "Huvudmenu", options);
    }

    Task ShowProductManagementMenuAsync()
    {
        var options = new[]
        {
            Option("Lista produkter", ListProductsAsync),
            Option("Lägg till produkt", CreateProductAsync),
            Option("Uppdatera produkt", UpdateProductAsync),
            Option("Ta bort produkt", DeleteProductAsync),
            Option("Tillbaka", ShowAdminMenuAsync)
        };
        return Menu.ShowMenu("Produktadministration", "Produkt hantering", options);
    }

    #endregion

    // Ta bort senare när jag har implementerat...
    void ShowOrders() => Console.WriteLine("Implementera...");
    void ShowStatistics() => Console.WriteLine("Implementera...");


    #region Admin funktioner
    async Task<List<User>> ListUsersAsync()
    {
        var all = await _db.Users
            .OrderBy(u =>  u.Id).ToListAsync();

        Console.WriteLine("Tillgängliga användare:");
        foreach(var u in all)
            Console.WriteLine($"{u.Id}: {u.Username}");
        Console.WriteLine();

        return all;
    }

    async Task<User?> SelectUserAsync(List<User> users)
    {
        Console.Write("Ange ID på användare att redigera (eller tomt för att avbryta): ");
        var line = Console.ReadLine()?.Trim();
        if (!int.TryParse(line, out var id))
            return null;

        var user = await _db.Users
            .Include(u => u.Authorities).FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            Console.WriteLine("Felaktigt ID");
            await Task.Delay(1000);
        }
        return user;
    }

    async Task EditRoleAsync(User user)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Redigerar roller för {user.Username}");
            Console.WriteLine();

            var allRoles = await _db.Authorities.ToListAsync();

            Console.WriteLine("Nuvarande roller:");
            foreach (var r in user.Authorities)
            {
                Console.WriteLine($" [X] {r.Name}");
            }
            Console.WriteLine();

            Console.WriteLine("Tillgängliga roller:");
            foreach(var r in allRoles.Except(user.Authorities, new AuthorityComparer()))
                Console.WriteLine($" [ ] {r.Name}");
            Console.WriteLine();

            Console.Write("Skriv '+Rollnamn' tex '+Admin' för att lägga till, '-Rollnamn' tex '-Admin' för att ta bort, eller tomt för klar: ");
            var cmd = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(cmd))
                break;

            var action = cmd[0];
            var roleName = cmd.Substring(1);

            var role = allRoles.FirstOrDefault(a => a.Name!.Equals(roleName, StringComparison.OrdinalIgnoreCase));

            if (role == null)
            {
                Console.WriteLine("Hittade ingen sådan roll.");
                await Task.Delay(1000);
                continue;
            }

            if(action == '+' && !user.Authorities.Contains(role))
                user.Authorities.Add(role);
            else if(action == '-' && user.Authorities.Contains(role))
                user.Authorities.Remove(role);

            await _db.SaveChangesAsync();
        }
    }

    async Task ManageUserRolesAsync()
    {
        Console.Clear();
        var users = await ListUsersAsync();
        if (users.Count == 0)
        {
            Console.WriteLine("Inga användare hittades");
            await Task.Delay(1000);
            return;
        }

        var user = await SelectUserAsync(users);
        if (user == null) return;

        await EditRoleAsync(user);
        Console.WriteLine("Uppdatering klar!");
        await Task.Delay(1000);
    }

    #endregion

    #region Produkt Funktioner

    async Task ListProductsAsync()
    {
        var products = await _productService.GetAllAsync();
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"ID │ Namn {new string(' ', 16)}│ Pris {new string(' ', 6)}│ Leverantör {new string(' ', 8)}│ Enhet {new string(' ', 5)}│ Lagersaldo");
        Console.WriteLine(new string('⎯', 90));
        Console.ResetColor();
        foreach (var p in products)
        {
            Console.WriteLine(new string('⎯', 90));
            Console.WriteLine($"{p.Id,-2} │ {p.Name,-20} │ {p.Price,-10} │ {p.Supplier?.Name,-18} │ {p.Unit?.Name,-10} │ {p.Storage}");
            Console.WriteLine(new string('⎯', 90));
        }
        Console.WriteLine();
        Console.WriteLine("Tryck valfri tangent för att återgå");
        Console.ReadKey(true);
        await ShowProductManagementMenuAsync();
    }

    async Task CreateProductAsync()
    {
        Console.Clear();
        Console.Write("Namn: ");
        var name = Console.ReadLine()?.Trim();
        Console.Write("EAN kod: ");
        var eanCode = Console.ReadLine()?.Trim();
        Console.Write("Artikel nummer: ");
        var artNumber = Console.ReadLine()?.Trim();
        Console.Write("Kategori: ");
        var category = Console.ReadLine()?.Trim();
        Console.Write("Antal: ");
        var totalStorage = int.Parse(Console.ReadLine()!);
        Console.Write("Beskrivning: ");
        var bio = Console.ReadLine()?.Trim();
        Console.Write("Pris: ");
        var price = decimal.Parse(Console.ReadLine()!);
        Console.Write("Leverantörs-ID: ");
        var supId = int.Parse(Console.ReadLine()!);
        Console.Write("Enhets-ID: ");
        var unitId = int.Parse(Console.ReadLine()!);

        var article = new Article
        {
            Name = name!,
            EanCode = eanCode!,
            ArticleCode = artNumber!,
            Category = category!,
            Storage = totalStorage,
            Bio = bio!,
            Price = price,
            SupplierId = supId,
            UnitId = unitId,
            DiscountId = 1
        };

        await _productService.AddAsync(article);
        Console.WriteLine("Produkten skapad!");
        Thread.Sleep(1000);
        await ShowProductManagementMenuAsync();
    }

    async Task UpdateProductAsync()
    {
        Console.Clear();
        Console.Write("Ange ID på produkt för att uppdatera: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Ogiltligt ID");
            Thread.Sleep(1000);
            await ShowProductManagementMenuAsync();
        }

        var existing = await _productService.GetByIdAsync(id);
        if (existing == null)
        {
            Console.WriteLine("Produkten hittades inte");
            Thread.Sleep(1000);
            await ShowProductManagementMenuAsync();
        }

        Console.Write($"Nytt namn [{existing!.Name}]: ");
        var name = Console.ReadLine()?.Trim();
        Console.Write($"Ny beskrivning [{existing.Bio}]: ");
        var bio = Console.ReadLine()?.Trim();
        Console.Write($"Nytt pris [{existing.Price}]: ");
        var priceText = Console.ReadLine();
        Console.Write($"Ny leverantörs-ID [{existing.SupplierId}]: ");
        var supText = Console.ReadLine();
        Console.Write($"Ny enhets-ID [{existing.UnitId}]: ");
        var unitText = Console.ReadLine();

        if(!string.IsNullOrWhiteSpace(name))
            existing.Name = name;
        if(!string.IsNullOrWhiteSpace(bio))
            existing.Bio = bio;
        if(decimal.TryParse(priceText, out var price))
            existing.Price = price;
        if (int.TryParse(supText, out var sup))
            existing.SupplierId = sup;
        if(int.TryParse(unitText, out var unit))
            existing.UnitId = unit;

        var ok = await _productService.UpdateAsync(existing);
        Console.WriteLine(ok ? "Produkt uppdaterad!" : "Uppdateringen misslyckades");
        Thread.Sleep(1000);
        await ShowProductManagementMenuAsync();

    }

    async Task DeleteProductAsync()
    {
        Console.Clear();
        Console.Write("Ange ID på produkt att ta bort: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Ogiltligt ID");
            Thread.Sleep(1000);
            await ShowProductManagementMenuAsync();
        }

        Console.Write("Är du säker? ENTER för ja | ESC för nej: ");
        ConsoleKeyInfo key = Console.ReadKey(true);

        switch (key.Key)
        {
            case ConsoleKey.Enter:
                var ok = await _productService.DeleteAsync(id);
                Console.WriteLine(ok ? "Produkten borttagen!" : "Bortttagning misslyckades");
                Thread.Sleep(1000);
                await ShowProductManagementMenuAsync();
                break;
            case ConsoleKey.Escape:
                await ShowProductManagementMenuAsync();
                break;
        }

        
    }

    #endregion

    #region Produkt Funktioner för användare

    Task ShowProductsAsync() => ShowProductsFilterMenuAsync();

    Task ShowProductsFilterMenuAsync()
    {
        var options = new[]
        {
            Option("Visa alla produkter", () => ListAndSelectProductsAsync(null)),
            Option("Stora", () => ListAndSelectProductsAsync("Stora")),
            Option("Medium", () => ListAndSelectProductsAsync("Medium")),
            Option("Små", () => ListAndSelectProductsAsync("Små")),
            Option("Tillbaka", ShowUserMenuAsync)
        };
        return Menu.ShowMenu("Produktmeny", "Välj kategori", options);
    }

    async Task ListAndSelectProductsAsync(string? category)
    {
        Console.Clear();
        List<Article> products;
        
        if(string.IsNullOrEmpty(category))
            products = await _productService.GetAllAsync();
        else
            products = await _productService.GetByCategoryAsync(category);

        Console.WriteLine(category == null ? "Alla produkter:" : $"Produkter - katergori: {category}");
        Console.WriteLine(new string('-', 40));

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id}: {product.Name} - {product.Price} kr");
        }
        Console.WriteLine();

        Console.Write("Ange produkt ID för detaljer (tomt för tillbaka): ");
        var txt = Console.ReadLine()?.Trim();
        if (int.TryParse(txt, out var id))
        {
            var prod = products.FirstOrDefault(x => x.Id == id);
            if (prod != null)
            {
                await ShowProductsDetailsAsync(prod);
            }
        }

        await ShowProductsFilterMenuAsync();

    }

    Task ShowProductsDetailsAsync(Article product)
    {
        var desc = new StringBuilder()
            .AppendLine($"Produkt: {product.Name}")
            .AppendLine($"Pris: {product.Price} kr")
            .AppendLine("Beskrivning:")
            .AppendLine(product.Bio)
            .AppendLine("Tryck ENTER för att lägga i kundvagnen.")
            .ToString().TrimEnd('\n');

        var lines = desc.Split('\n');
        int longest = lines.Max(i => i.Length);
        int minWidth = Math.Max(longest + 4, 40);
        int boxHeight = lines.Length + 5;

        var options = new[]
        {
            Option(desc, () => _cartService.AddToCart(product)),
            Option("Tillbaka till lista", ShowProductsFilterMenuAsync)
        };
        return Menu.ShowMenu("Produktdetaljer", "Produktinformation", options, minBoxWidth: minWidth, boxHeight: boxHeight);
    }

    async Task ShowCartAsync()
    {
        Console.Clear();
        var items = _cartService.GetItems();

        if (!items.Any())
        {
            Console.WriteLine("Din kundvagn är tom");
            await Task.Delay(1000);
            await ShowUserMenuAsync();
            return;
        }

        Console.WriteLine("Din kundvang:");
        Console.WriteLine(new string('-', 40));

        foreach (var item in items)
        {
            var lineTotal = item.Article.Price * item.Quantity;
            Console.WriteLine($"{item.Article.Name,-20} x{item.Quantity,2} = {lineTotal,6} kr");
        }
        Console.WriteLine(new string('-', 40));
        var total = items.Sum(i => i.Article.Price * i.Quantity);
        Console.WriteLine($"Totalt: {total} kr");
        Console.WriteLine();

        var options = new[]
        {
            Option("Betala", async () =>
            {
                if(_currentUser == null)
                    return;
                var ok = await _cartService.CheckoutAsync(_currentUser);

                if(ok == null)
                {
                    Console.WriteLine("Betalningen misslyckades");
                    await Task.Delay(1000);
                    await ShowUserMenuAsync();
                    return;
                }

                await ShowPaymentMethodMenuAsync(ok!);
            }),
            Option("Tillbaka", ShowUserMenuAsync)
        };

        await Menu.ShowMenu("Kundvagn", "Kundvagn", options);
    }



    #endregion

    #region Order Funktioner

    async Task ManageOrderAsync()
    {
        Console.Clear();
        var orders = await _orderService.GetAllOrdersAsync();
        Console.WriteLine("Alla ordrar:");
        Console.WriteLine("Id | Datum | Användare | Totalt | Betalning");

        foreach(var o in orders)
        {
            var paid = o.Payment != null ? o.Payment.Method?.Name : "Ej betald";
            Console.WriteLine($"{o.Id, -3}|{o.OrderDate:g}|{o.User!.Username, -12}|{o.TotalAmount,8} kr |{paid}");
        }
        Console.WriteLine();
        Console.Write("Ange order-Id att redigera (tomt för att gå tillbaka): ");
        var input = Console.ReadLine()?.Trim();
        if (int.TryParse(input, out var id))
        {
            var order = await _orderService.GetByIdAsync(id);
            if(order != null)
                await ShowOrderDetailsAsync(order, isAdmin: true);
        }
        await ShowAdminMenuAsync();
    }

    async Task ShowMyOrdersAsync()
    {
        Console.Clear();
        if(_currentUser == null)
            return;

        var orders = await _orderService.GetOrdersByUserIdAsync(_currentUser.Id);
        Console.WriteLine("Dina ordrar:");
        foreach (var o in orders)
        {
            Console.WriteLine($"{o.Id}: {o.OrderDate:g} - {o.TotalAmount} kr");
        }
        Console.WriteLine();
        Console.Write("Ange id för att få en detalirad order (tomt för att gå tillbaka): ");
        var input = Console.ReadLine()?.Trim();
        if(int.TryParse(input, out var id))
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order != null)
                await ShowOrderDetailsAsync(order, isAdmin: false);
        }

        await ShowUserMenuAsync();
    }

    async Task ShowOrderDetailsAsync(Order order, bool isAdmin)
    {
        Console.Clear();
        Console.WriteLine($"Order {order.Id} - {order.OrderDate:g}");
        Console.WriteLine($"Användare: {order.User.Username} - {order.User.Name}");
        Console.WriteLine($"Adress: {order.User.Address}, {order.User.Street}, {order.User.City}");
        Console.WriteLine($"Telefon: {order.User.PhoneNumber}, E-post: {order.User.Email}");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine("Produkter:");
        foreach (var item in order.Items)
        {
            Console.WriteLine($"{item.Article.Name} x{item.Quantity} - {item.UnitPrice} kr = {item.PriceAtPurchase} kr");
        }
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Total: {order.TotalAmount} kr");

        var paymentInfo = order.Payment != null ? $"{order.Payment.Method.Name} - {order.Payment.Amount}" : "Ej betald";

        Console.WriteLine($"Betalning: {paymentInfo}");
        Console.WriteLine();
        var options = new List<(string, Action)>();
        if (isAdmin)
        {
            Console.WriteLine();
            Console.WriteLine("Vill du uppdatera ordern? Tryck Enter om JA");
            var input = Console.ReadKey().Key;
            if (input == ConsoleKey.Enter)
                await EditOrderAsync(order);
            else
                await ShowAdminMenuAsync();

        }
        else
        {
            Console.WriteLine("Tryck på valfri tagent för att gå tillbaka");
            Console.ReadKey(true);
            await ShowUserMenuAsync();
        }
    }

    async Task EditOrderAsync(Order order)
    {
        Console.Clear();
        Console.Write($"Nytt totalbelopp [{order.TotalAmount}]: ");
        var input = Console.ReadLine()?.Trim();
        if(decimal.TryParse(input, out var total))
            order.TotalAmount = total;
        else
            Console.WriteLine("Ogiltligt belopp");

        var ok = await _orderService.UpdateOrderAsync(order);
        Console.WriteLine(ok ? "Order uppdaterad!" : "Uppdatering misslyckades");
        await Task.Delay(1000);
        await ShowOrderDetailsAsync(order, isAdmin: true);
    }

    #endregion

    async Task ShowPaymentMethodMenuAsync(Order order)
    {
        Console.Clear();
        var methods = await _paymentService.GetAllMethodsAsync();

        Console.WriteLine($"Totalt att betala: {order.TotalAmount} kr\nVälj betalningsmetod:");

        for (int i = 0; i < methods.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {methods[i].Name}");
        }
        Console.Write("\nAnge nummer: ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out var index) && index > 0 && index <= methods.Count)
        {
            var method = methods[index - 1];
            var payment = await _paymentService.CreatePaymentAsync(order.Id, method.Id, order.TotalAmount);
            order.PaymentId = payment.Id + 1;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();

            Console.WriteLine($"\nBetalning med '{method.Name}' genomförd!");
            await Task.Delay(1000);
        }
        else
        {
            Console.WriteLine("Ogiltligt val");
            await Task.Delay(1000);
        }

        await ShowUserMenuAsync();
    }
}
class AuthorityComparer : IEqualityComparer<Authority>
{
    public bool Equals(Authority? x, Authority? y) =>
        x != null && y != null && x.Id == y.Id;

    public int GetHashCode(Authority obj) => obj.Id;
}


