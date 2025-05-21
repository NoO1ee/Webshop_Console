using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class AuthService
{
    readonly MyDbContext _db;
    public AuthService(MyDbContext db) => _db = db;

    private static string ReadPassword()
    {
        var sb = new StringBuilder();
        ConsoleKey key;
        while ((key = Console.ReadKey(true).Key) != ConsoleKey.Enter)
        {
            if (key == ConsoleKey.Backspace && sb.Length > 0)
            {
                sb.Length--;
                Console.Write("\b \b");
            }else if (!char.IsControl((char)key))
            {
                sb.Append((char)key);
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return sb.ToString();

    }

    private static string Hash(string text)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToBase64String(bytes);
    }

    public async Task<User?> LoginAsync()
    {
        Console.Clear();
        var username = Prompt("Ange användarnamn: ");
        var password = Prompt("Ange lösenord: ", hideInput: true);

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Användarnamn och lösenord får inte vara tomma");
            await Task.Delay(1000);
            return null;
        }

        var hashed = Hash(password);
        var user = await _db.Users
            .Include(u => u.Authorities)
            .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashed);

        if (user == null)
        {
            Console.WriteLine("Felaktigt användarnamn eller lösenord");
            await Task.Delay(1000);
            return null;
        }

        Console.WriteLine($"Välkommen {user.Username}!\n");
        await Task.Delay(1000);
        return user;
    }

    private string? Prompt(string message, bool hideInput = false)
    {
        Console.Write(message);
        return hideInput ? ReadPassword() : Console.ReadLine()?.Trim();
    }

    private async Task<Authority?> GetRoleAsync(string roleName)
    {
        var norm = roleName.ToLower();
        return await _db.Authorities.FirstOrDefaultAsync(a => a.Name.ToLower() == norm);
    }

    public async Task RegisterAsync()
    {
        Console.Clear();
        var username = Prompt("Välj användarnamn: ");
        var password = Prompt("Välj lösenord: ", hideInput: true);

        Console.Write("Förnamn och Efternamn: ");
        var name = Console.ReadLine()?.Trim();

        Console.Write("Adress: ");
        var address = Console.ReadLine()?.Trim();

        Console.Write("Postnummer: ");
        var street = Console.ReadLine()?.Trim();

        Console.Write("Stad: ");
        var city = Console.ReadLine()?.Trim();

        Console.Write("Telefonnummer: ");
        var phone = Console.ReadLine()?.Trim();

        Console.Write("E-post: ");
        var email = Console.ReadLine()?.Trim();

        Console.Write("Ålder: ");
        var country = Console.ReadLine()?.Trim();


        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(country))
        {
            Console.WriteLine("Du måste fylla i allt.");
            await Task.Delay(1000);
            return;
        }

        if (await _db.Users.AnyAsync(u => u.Username == username) || await _db.Users.AnyAsync(u => u.PhoneNumber == phone))
        {
            Console.WriteLine("Användarnamnet finns redan eller telefonnummer finns redan");
            await Task.Delay(1000);
            return;
        }

        var userRole = await GetRoleAsync("User");
        if (userRole is null)
        {
            Console.WriteLine("Kunde inte hitta rollen User i databasen");
            await Task.Delay(1000);
            return;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = Hash(password),
            Name = name,
            Address = address,
            Street = street,
            City = city,
            PhoneNumber = phone,
            Email = email,
            Age = int.TryParse(country, out var age) ? age : null,
            Authorities = new List<Authority> { userRole }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        Console.WriteLine("Registering lyckades!");
        await Task.Delay(1000);

    }
}
