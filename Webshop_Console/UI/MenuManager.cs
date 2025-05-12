using System;
using System.Collections.Generic;
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

    public MenuManager(AuthService auth, MyDbContext db)
    {
        _auth = auth;
        _db = db;
    }

    public async Task ShowMainMenuAsync()
    {
        var options = new[]
        {
            Option("Log in", HandleLoginAsync),
            Option("Register", _auth.RegisterAsync),
            Option("Exit", () => Environment.Exit(0))
        };
        await Menu.ShowMenu("Duck4Hire", "Log in / Register", options);
    }

    // Kör asynkrona actions i en sync action
    void RunAsync(Func<Task> func) => Task.Run(func).Wait();

    // För att slå ihop label och Func<Task> till string, action
    (string, Action) Option(string label, Func<Task> action) => (label, () => RunAsync(action));

    (string, Action) Option(string label, Action action) => (label, action);

    (string, Action) LogoutOption() => Option("Logga ut", () => ShowMainMenuAsync().Wait());

    async Task HandleLoginAsync()
    {
        var user = await _auth.LoginAsync();
        if (user != null)
            await RouteByRoleAsync(user);
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
        var options = new List<(string, Action)>
        {
            Option("Se statistik", ShowStatistics),
            Option("Hantera användares roller", ManageUserRoles),
            LogoutOption()
        };

        return Menu.ShowMenu("Ägarpanel", "Ägarpanel", options.ToArray());
    }

    Task ShowUserMenuAsync()
    {
        var options = new[]
        {
            Option("Visa produkter", ShowProducts),
            LogoutOption()
        };
        return Menu.ShowMenu("Användarpanel", "Huvudmenu", options);
    }

    // Ta bort senare när jag har implementerat...
    void ShowOrders() => Console.WriteLine("Implementera...");
    void ShowStatistics() => Console.WriteLine("Implementera...");
    void ShowProducts() => Console.WriteLine("Implementera...");

    void ManageUserRoles()
    {
        //Lista alla användare
        //Låt ägare välja användare
        //Hämta användare (Authorities)
        //Visa roller. lägga til eller ta bort.
        Console.WriteLine("Implementera...");
    }

}
