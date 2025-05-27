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
    readonly LoginHandler _auth;
    readonly MyDbContext _db;
    readonly ProductHandler _productService;
    readonly CartHandler _cartService;
    readonly PaymentHandler _paymentService;
    readonly List<ArticleModel> _cart = new List<ArticleModel>();
    readonly OrderHandler _orderService;

    UserModel? _currentUser;

    public MenuManager(LoginHandler auth, MyDbContext db, ProductHandler productService)
    {
        _auth = auth;
        _db = db;
        _productService = productService;
        _cartService = new CartHandler(db);
        _paymentService = new PaymentHandler(db);
        _orderService = new OrderHandler(db);
    }


    #region Main funktioner
    // Kör asynkrona actions i en sync action
    void RunAsync(Func<Task> func) => Task.Run(func).Wait();

    //Kombinerar label och Func<Task> till en menyoption
    (string, Action) Option(string label, Func<Task> action) => (label, () => RunAsync(action));
    (string, Action) Option(string label, Action action) => (label, action);

    (string, Action) LogoutOption() => Option("Logga ut", () => ShowMainMenuAsync().Wait());

    async Task HandleLoginAsync()
    {
        _currentUser = await _auth.LoginAsync();
        if (_currentUser != null)
            await RouteByRoleAsync(_currentUser);
    }

    async Task RouteByRoleAsync(UserModel user)
    {
        if (user.Authorities.Any(r => r.IsOwner))
            await ShowOwnerMenuAsync(user);
        else if (user.Authorities.Any(r => r.IsAdmin))
            await ShowAdminMenuAsync();
        else
            await ShowUserMenuAsync();
    }
    #endregion

    #region Menyer

    //Huvud meny
    public async Task ShowMainMenuAsync()
    {
        var options = new[]
        {
            Option("Logga in", HandleLoginAsync),
            Option("Registrera", _auth.RegisterAsync),
            Option("Avsluta", () => Environment.Exit(0))
        };
        await Menu.ShowMenu("Duck4Hire", "Logga in / Registrera", options: options, titleColor: ConsoleColor.Yellow, textColor: ConsoleColor.White, selectedColor: ConsoleColor.Green, minBoxWidth: 30, boxHeight: 3);
    }

    //Admin meny
    Task ShowAdminMenuAsync()
    {
        var options = new[]
        {
            Option("Visa ordrar", ManageOrderAsync),
            Option("Hantera Artiklar", ShowProductManagementMenuAsync),
            Option("Visa produkter", ShowProductsAsync),
            Option("Visa kundvagn", ShowCartAsync),
            LogoutOption()
        };
        return Menu.ShowMenu("Adminpanel", "Adminpanel", options);
    }

    //Ägar meny
    Task ShowOwnerMenuAsync(UserModel owner)
    {
        var options = new []
        {
            Option("Visa ordrar", ManageOrderAsync),
            Option("Hantera Artiklar", ShowProductManagementMenuAsync),
            Option("Hantera användares roller", ManageUserRolesAsync),
            Option("Hantera Kategorier", ManageCategoryAsync),
            Option("Hantera Betalningsmetoder", ManagePaymentMethodsAsync),
            LogoutOption()
        };
        return Menu.ShowMenu("Ägarpanel", "Ägarpanel", options);
    }

    Task ShowUserMenuAsync()
    {
        var featured = _productService.GetAllAsync().Result.Where(p => p.IsFeatured).Take(3).ToList();

        var options = new List<(string, Action)>();
        var featuredText = new StringBuilder()
            .AppendLine("Utvalda produkter:");
        foreach (var p in featured)
        {
            featuredText.Clear();
            featuredText.AppendLine($"{p.Name} - {p.Price} kr");
            featuredText.AppendLine(p.Bio.Length > 50 ? p.Bio.Substring(0, 50) + "..." : p.Bio);
            options.Add(Option(featuredText.ToString(), () => ShowProductsDetailsAsync(p)));
        }

        
        options.Add(Option("Visa produkter", ShowProductsAsync));
        options.Add(Option("Visa kundvagn", ShowCartAsync));
        options.Add(Option("Redigera kontaktuppgifter", ManageProfileAsync));
        options.Add(Option("Se mina ordrar", ShowMyOrdersAsync));
        options.Add(LogoutOption());
        return Menu.ShowMenu("Användarpanel", "DeDuckers", options.ToArray());
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


    #region Admin funktioner
    async Task<List<UserModel>> ListUsersAsync()
    {
        var all = await _db.Users
            .OrderBy(u =>  u.Id).ToListAsync();

        Console.WriteLine("Tillgängliga användare:");
        foreach(var u in all)
            Console.WriteLine($"{u.Id}: {u.Username}");
        Console.WriteLine();

        return all;
    }

    async Task<UserModel?> SelectUserAsync(List<UserModel> users)
    {
        Console.Write("Vilken användare vill du redigera? Ange ID eller lämna det tomt för att gå tillbaka: ");
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

    async Task EditRoleAsync(UserModel user)
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
        var salesByProduct = await _productService.GetSalesBuProductAsync();

        var sorted = products.OrderByDescending(p => salesByProduct.TryGetValue(p.Id, out var count) ? count : 0).ToList();

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"ID │ Namn {new string(' ', 16)}│ Pris {new string(' ', 6)}│ Leverantör {new string(' ', 8)}│ Enhet {new string(' ', 5)}│ Lagersaldo {new string(' ', 5)}│ Antal sålda");
        Console.WriteLine(new string('⎯', 100));
        Console.ResetColor();
        foreach (var p in sorted)
        {
            salesByProduct.TryGetValue(p.Id, out var salesCount);
            Console.WriteLine(new string('⎯', 100));
            Console.WriteLine($"{p.Id,-2} │ {p.Name,-20} │ {p.Price,-10} │ {p.Supplier?.Name,-18} │ {p.Unit?.Name,-10} │ {p.Storage,-10} │ {salesCount,5} st");
            Console.WriteLine(new string('⎯', 100));
        }
        Console.WriteLine();
        Console.WriteLine("Tryck valfri tangent för att återgå");
        Console.ReadKey(true);
        await ShowProductManagementMenuAsync();
    }

    async Task CreateProductAsync()
    {
        var categories = await _productService.GetAllCategoriesAsyc();
        var suppliers = await _db.Suppliers.OrderBy(s => s.Id).ToListAsync();
        var units = await _db.Units.OrderBy(u => u.Id).ToListAsync();

        Console.Clear();
        Console.Write("Namn: ");
        var name = Console.ReadLine()?.Trim();
        Console.Write("EAN kod: ");
        var eanCode = Console.ReadLine()?.Trim();
        Console.Write("Artikel nummer: ");
        var artNumber = Console.ReadLine()?.Trim();
        //Console.Write("Kategori: ");
        //var category = Console.ReadLine()?.Trim();
        Console.Write("Antal: ");
        var totalStorage = int.Parse(Console.ReadLine()!);
        Console.Write("Beskrivning: ");
        var bio = Console.ReadLine()?.Trim();
        Console.Write("Pris: ");
        var price = decimal.Parse(Console.ReadLine()!);

        Console.WriteLine("Tillgänliga Kategorier:");
        categories.OrderBy(c => c.Id).ToList().ForEach(categories => Console.WriteLine($"{categories.Id}: {categories.Name}"));
        Console.Write("Katergori ID: ");
        var categoryId = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Tillgängliga leverantörer:");
        suppliers.OrderBy(s => s.Id).ToList().ForEach(s => Console.WriteLine($"{s.Id}: {s.Name}"));
        Console.Write("Leverantörs-ID: ");
        var supId = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Tillgängliga enheter:");
        units.OrderBy(e => e.Id).ToList().ForEach(u => Console.WriteLine($"{u.Id}: {u.Name}"));
        Console.Write("Enhets-ID: ");
        var unitId = int.Parse(Console.ReadLine()!);

        Console.Write("Visas startsidan? (Y/N): ");
        var showOnStart = Console.ReadKey(true).Key == ConsoleKey.Y;

        

        var article = new ArticleModel
        {
            Name = name!,
            EanCode = eanCode!,
            ArticleCode = artNumber!,
            CategoryId = categoryId,
            Storage = totalStorage,
            Bio = bio!,
            Price = price,
            SupplierId = supId,
            UnitId = unitId,
            DiscountId = 1,
            IsFeatured = showOnStart
        };

        await _productService.AddAsync(article);
        Console.WriteLine("Produkten skapad!");
        await Task.Delay(1000);
        await ShowProductManagementMenuAsync();
    }

    async Task UpdateProductAsync()
    {
        Console.Clear();
        Console.Write("Vilken produkt vill du uppdatera? Ange ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Ogiltligt ID");
            await Task.Delay(1000);
            await ShowProductManagementMenuAsync();
        }

        var existing = await _productService.GetByIdAsync(id);
        if (existing == null)
        {
            Console.WriteLine("Produkten hittades inte");
            await Task.Delay(1000);
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
        Console.Write($"nVisas på startsidan? (Y/N) [{existing.IsFeatured}]: ");
        var showOnStart = Console.ReadKey(true).Key == ConsoleKey.Y;

        if (!string.IsNullOrWhiteSpace(name))
            existing.Name = name;
        if(!string.IsNullOrWhiteSpace(bio))
            existing.Bio = bio;
        if(decimal.TryParse(priceText, out var price))
            existing.Price = price;
        if (int.TryParse(supText, out var sup))
            existing.SupplierId = sup;
        if(int.TryParse(unitText, out var unit))
            existing.UnitId = unit;
        existing.IsFeatured = showOnStart;

        var ok = await _productService.UpdateAsync(existing);
        Console.WriteLine(ok ? "Produkt uppdaterad!" : "Uppdateringen misslyckades");
        await Task.Delay(1000);
        await ShowProductManagementMenuAsync();

    }

    async Task DeleteProductAsync()
    {
        Console.Clear();
        Console.Write("Vilken produkt vill du ta bort? Ange ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Ogiltligt ID");
            await Task.Delay(1000);
            await ShowProductManagementMenuAsync();
        }

        Console.Write("Vill du verkligen ta bort produkten? Tryck Enter för att bekräfta, eller ESC för att abryta:  ");
        ConsoleKeyInfo key = Console.ReadKey(true);

        switch (key.Key)
        {
            case ConsoleKey.Enter:
                var ok = await _productService.DeleteAsync(id);
                Console.WriteLine(ok ? "Produkten borttagen!" : "Bortttagning misslyckades");
                await Task.Delay(1000);
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

    async Task ShowProductsFilterMenuAsync()
    {
        var categories = _productService.GetAllCategoriesAsyc();

        var options = new List<(string, Action)>();
        
        options.Add(Option("Visa alla produkter", () => ListAndSelectProductsAsync(null)));
        foreach (var c in categories.Result.OrderBy(x => x.Id))
            options.Add(Option(c.Name, () => ListAndSelectProductsAsync(c.Id)));

        options.Add(Option("Tillbaka", ShowUserMenuAsync));

        await Menu.ShowMenu("Produktmeny", "Välj kategori", options.ToArray());

    }

    async Task ListAndSelectProductsAsync(int? category)
    {
        Console.Clear();
        List<ArticleModel> products;
        
        if(category.HasValue)
            products = await _productService.GetByCategoryIdAsync(category.Value);
        else
            products = await _productService.GetAllAsync();

        Console.WriteLine(category.HasValue ? $"Produkter - katergori: {category.Value}" : "Alla produkter:");
        Console.WriteLine(new string('-', 40));

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id}: {product.Name} - {product.Price} kr");
        }
        Console.WriteLine();

        Console.Write("Ange produkt ID eller namn för detaljer (tomt för tillbaka): ");
        var txt = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(txt))
        {
            ArticleModel? product = null;
            if(int.TryParse(txt, out int id))
                product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                product = products.FirstOrDefault(x => x.Name.IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0);

            if (product != null)
            {
                await ShowProductsDetailsAsync(product);
                return;
            }
            else
            {
                Console.WriteLine("Hittade inge produkt.");
                await Task.Delay(1000);
            }
        }

        await ShowProductsFilterMenuAsync();

    }

    Task ShowProductsDetailsAsync(ArticleModel product)
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

        await Task.Delay(2000);

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

    async Task ShowOrderDetailsAsync(OrderModel order, bool isAdmin)
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

    async Task EditOrderAsync(OrderModel order)
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

    #region Redigera kuppgifter

    async Task ManageProfileAsync()
    {
        Console.Clear();
        if (_currentUser == null)
        {
            Console.WriteLine("Ingen användare inloggad.");
            await Task.Delay(1000);
            await ShowUserMenuAsync();
            return;
        }

        var user = await _db.Users.FindAsync(_currentUser.Id);
        if(user == null)
        {
            Console.WriteLine("Ingen användare hittades i databaseb.");
            await Task.Delay(1000);
            await ShowUserMenuAsync();
            return;
        }

        Console.WriteLine("Redigera din kontaktuppgifter. Lämna tomt för att behålla nuvarande");
        Console.WriteLine();

        Console.Write($"Name [{user.Name}]: ");
        var name = Console.ReadLine()?.Trim();
        Console.Write($"E-post [{user.Email}]: ");
        var email = Console.ReadLine()?.Trim();
        Console.Write($"Telefon [{user.PhoneNumber}]: ");
        var phone = Console.ReadLine()?.Trim();
        Console.Write($"Adress [{user.Address}]: ");
        var address = Console.ReadLine()?.Trim();
        Console.Write($"Postnummer [{user.Street}]: ");
        var city = Console.ReadLine()?.Trim();
        Console.Write($"Stad [{user.City}]: ");
        var postNumber = Console.ReadLine()?.Trim();
        Console.Write($"Ålder [{user.Age}]: ");
        var age = Console.ReadLine()?.Trim();

        if(!string.IsNullOrWhiteSpace(name))
            user.Name = name;
        if(!string.IsNullOrWhiteSpace(email))
            user.Email = email;
        if(!string.IsNullOrWhiteSpace(phone))
            user.PhoneNumber = phone;
        if(!string.IsNullOrWhiteSpace(address))
            user.Address = address;
        if(!string.IsNullOrWhiteSpace(city))
            user.City = city;
        if(!string.IsNullOrWhiteSpace(postNumber))
            user.Street = postNumber;
        if(int.TryParse(age, out var a))
            user.Age = a;

        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        _currentUser = user;

        Console.WriteLine("Dina uppgifter har uppdaterats.");
        await Task.Delay(1000);
        await ShowUserMenuAsync();

    }

    #endregion

    #region Betalnings funktioner
    async Task ShowPaymentMethodMenuAsync(OrderModel order)
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

    async Task ManagePaymentMethodsAsync()
    {
        Console.Clear();
        var methods = await _paymentService.GetAllMethodsAsync();
        Console.WriteLine("Betalningsmetoder:");
        foreach(var m in methods)
        {
            Console.WriteLine($"{m.Id}: {m.Name}");
        }
        Console.WriteLine();
        Console.WriteLine("A = lägg till, U = uppdatera, D = Ta bort, Enter = Tillbaka");
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.A:
                await AddPaymentMethodAsync();
                break;
            case ConsoleKey.U:
                await UpdatePaymentMethodAsync(methods);
                break;
            case ConsoleKey.D:
                await DeletePaymentMethodAsync(methods);
                break;
            case ConsoleKey.Enter:
                await ShowOwnerMenuAsync(_currentUser!);
                break;
        }
        await ManagePaymentMethodsAsync();
    }

    async Task ManageCategoryAsync()
    {
        Console.Clear();
        var category = await _productService.GetAllCategoriesAsyc();
        Console.WriteLine("Kategorier:");
        foreach (var m in category)
        {
            Console.WriteLine($"{m.Id}: {m.Name}");
        }
        Console.WriteLine();
        Console.WriteLine("A = lägg till, U = uppdatera, D = Ta bort, Enter = Tillbaka");
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.A:
                await AddCategoryAsync();
                break;
            case ConsoleKey.U:
                await UpdateCategoryAsync(category);
                break;
            case ConsoleKey.D:
                await DeleteCategoryAsync(category);
                break;
            case ConsoleKey.Enter:
                await ShowOwnerMenuAsync(_currentUser!);
                break;
        }
        await ManageCategoryAsync();
    }


    async Task AddPaymentMethodAsync()
    {
        Console.Clear();
        Console.Write("Nytt namn på betalningsmetod: ");
        string name = Console.ReadLine()?.Trim()!;
        if(!string.IsNullOrWhiteSpace(name))
            await _paymentService.CreatePaymentMethodAsync(name!);
        else
            Console.WriteLine("Ogiltligt namn");
    }

    async Task UpdatePaymentMethodAsync(List<PaymentMethod> methods)
    {
        Console.Clear();
        Console.Write("Vilken metod skulle fu vilja ändra? ID: ");
        if(int.TryParse(Console.ReadLine(), out var id))
        {
            var method = methods.FirstOrDefault(m => m.Id == id);
            if (method != null)
            {
                Console.Write($"Nytt namn [{method.Name}]: ");
                var name = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(name))
                    method.Name = name;
                else
                    Console.WriteLine("Ogiltligt namn");
            }
            else
                Console.WriteLine("Ingen sådan metod hittades");
        }
    }

    async Task DeletePaymentMethodAsync(List<PaymentMethod> methods)
    {
        Console.Clear();
        Console.Write("Vilken metod vill du ta bort? Ange id: ");
        if (int.TryParse(Console.ReadLine(), out var id))
        {
            var method = methods.FirstOrDefault(m => m.Id == id);
            if (method != null)
            {
                Console.Write($"Är du säker på att du vill ta bort '{method.Name}'? (Y/N): ");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                    await _paymentService.DeletePaymentMethodAsync(method.Id);
            }
            else
                Console.WriteLine("Ingen sådan metod hittades");
        }
    }

    #endregion

    #region Kategori funktioner

    async Task AddCategoryAsync()
    {
        Console.Clear();
        Console.Write("Kategori namn: ");
        var name = Console.ReadLine()?.Trim();
        if(!string.IsNullOrEmpty(name))
            await _productService.CreateCategoryASync(name!);
        else
            Console.WriteLine("Ogitligt namn");
    }

    async Task UpdateCategoryAsync(List<CategoryModel> list)
    {
        Console.Clear();
        Console.Write("Vilket id vill du ändra på?: ");
        if (int.TryParse(Console.ReadLine(), out var id))
        {
            var category = list.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                Console.Write($"Nytt namn [{category.Name}]: ");
                var name = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(name))
                    category.Name = name;
                else
                    Console.WriteLine("Ogiltigt namn");
            }
            else
                Console.WriteLine("Ingen sådan kategori hittades");
        }
    }

    async Task DeleteCategoryAsync(List<CategoryModel> list)
    {
        Console.Clear();
        Console.Write("Vilket ID vill du ta bort?: ");

        if(int.TryParse(Console.ReadLine(), out var id))
        {
            var category = list.FirstOrDefault(c => c.Id == id);
            if(category != null)
            {
                Console.Write($"Är du säker på att du vill ta bort '{category.Name}'? (Y/N): ");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                    await _productService.DeleteCategoryAsync(category.Id);
            }
            else
                Console.WriteLine("Ingen sådan kategori hittades");
        }
    }
    
    #endregion
}
class AuthorityComparer : IEqualityComparer<AuthorityModel>
{
    public bool Equals(AuthorityModel? x, AuthorityModel? y) =>
        x != null && y != null && x.Id == y.Id;

    public int GetHashCode(AuthorityModel obj) => obj.Id;
}


