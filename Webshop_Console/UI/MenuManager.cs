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
        await Menu.ShowMenu("Duck4Hire", "Log in / Register", new (string, Action)[]
        {
            ("Log in", () => RunAsync(async () => await HandleLogin())),
            ("Register", () => RunAsync(async () => { await _auth.RegisterAsync(); })),
            ("Exit", () => Environment.Exit(0))
        });
    }

    void RunAsync(Func<Task> func) => Task.Run(func).Wait();

    async Task HandleLogin()
    {
        var user = await _auth.LoginAsync();
        if (user != null)
            await RouteByRoleAsync(user);
    }

    async Task RouteByRoleAsync(User user)
    {
        if (user.Authoriries.Any(r => r.IsAdmin))
            await ShowAdminMenu();
        else if (user.Authoriries.Any(r => (bool)r.IsOwner!))
            await ShowOwnerMenu();
        else
            await ShowUserMenu();
    }

    Task ShowAdminMenu() => Menu.ShowMenu("Adminpanel", "Välj", new (string, Action)[]
    {
        ("Se ordrar", () => Console.WriteLine("Implementera....")),
        ("Logga ut", () => ShowMainMenuAsync().Wait())
    });

    Task ShowOwnerMenu() => Menu.ShowMenu("Ägarpanel", "Välj", new (string, Action)[]
    {
        ("Se statestik", () => Console.WriteLine("Implementera.....")),
        ("Ändra titel i main menu", () => Console.WriteLine("Implementera.....")),
        ("Logga ut", () => ShowMainMenuAsync().Wait())
    });

    Task ShowUserMenu() => Menu.ShowMenu("Användarpanel", "Välj", new (string, Action)[]
    {
        ("Visa produkter", () => Console.WriteLine("Implmentera....")),
        ("Logga ut", () => ShowMainMenuAsync().Wait())
    });

}
