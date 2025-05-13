using Microsoft.EntityFrameworkCore;
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

    User? _currentUser;

    public MenuManager(AuthService auth, MyDbContext db)
    {
        _auth = auth;
        _db = db;
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
            Option("Se ordrar", ShowOrders),
            LogoutOption()
        };
        return Menu.ShowMenu("Adminpanel", "Adminpanel", options);
    }

    Task ShowOwnerMenuAsync(User owner)
    {
        var options = new []
        {
            Option("Se statistik", ShowStatistics),
            Option("Hantera användares roller", ManageUserRolesAsync),
            LogoutOption()
        };

        return Menu.ShowMenu("Ägarpanel", "Ägarpanel", options);
    }

    Task ShowUserMenuAsync()
    {
        var options = new[]
        {
            Option("Visa produkter", ShowProducts),
            LogoutOption()
        };
        Console.WriteLine("Test");
        return Menu.ShowMenu("Användarpanel", "Huvudmenu", options);
    }

    #endregion

    // Ta bort senare när jag har implementerat...
    void ShowOrders() => Console.WriteLine("Implementera...");
    void ShowStatistics() => Console.WriteLine("Implementera...");
    void ShowProducts() => Console.WriteLine("Implementera...");

    void AddToCart(Article product)
    {
        if (_currentUser == null)
            throw new InvalidOperationException("Ingen användare inloggad.");

        var order = _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.UserId == _currentUser.Id && o.Payment == null);

        if (order == null)
        {
            order = new Order
            {
                UserId = _currentUser.Id,
                OrderDate = DateTime.Now,
                Items = new List<OrderItem>()
            };
            _db.Orders.Add(order);
        }

        var existting = order.Items.FirstOrDefault(i => i.ArticleId == product.Id);
        if (existting != null)
        {
            existting.Quantity++;
        }
        else
        {
            order.Items.Add(new OrderItem
            {
                ArticleId = product.Id,
                Quantity = 1,
                UnitPrice = product.Price,
            });
        }

        _db.SaveChanges();
    }

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

            var role = allRoles.FirstOrDefault(a => a.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

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

  
}
class AuthorityComparer : IEqualityComparer<Authority>
{
    public bool Equals(Authority? x, Authority? y) =>
        x != null && y != null && x.Id == y.Id;

    public int GetHashCode(Authority obj) => obj.Id;
}


