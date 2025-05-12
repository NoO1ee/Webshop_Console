using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class AuthService
{
    readonly MyDbContext _db;
    public AuthService(MyDbContext db) => _db = db;

    string ReadPassword()
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

    string Hash(string text)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToBase64String(bytes);
    }

    public async Task<User?> LoginAsync()
    {
        Console.Clear();
        Console.Write("Ange användarnamn: ");
        var username = Console.ReadLine();

        Console.Write("Ange lösenord: ");
        var password = ReadPassword();

        var hashed = Hash(password);
        var user = await _db.Users
            .Include(u => u.Authoriries)
            .FirstOrDefaultAsync(u => u.UserName == username && u.Password == hashed);

        if (user == null)
        {
            Console.WriteLine("Felaktigt användarnamn eller lösenord");
            await Task.Delay(1000);
            return null;
        }

        Console.WriteLine($"Välkommen {user.UserName}!\n");
        return user;
    }

    public async Task RegisterAsync()
    {
        Console.Clear();
        Console.Write("Välj användarnamn: ");
        var username = Console.ReadLine();
        Console.Write("Välj lösenord: ");
        var password = ReadPassword();

        if (await _db.Users.AnyAsync(u => u.UserName == username))
        {
            Console.WriteLine("Användarnamnet finns redan");
            await Task.Delay(1000);
            return;
        }

        var user = new User
        {
            UserName = username,
            Password = Hash(password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var assignment = new Authority
        {
            UserId = user.Id,
            Name = user.UserName!,
            IsAdmin = false,
            IsOwner = false,
        };

        _db.Authorities.Add(assignment);
        await _db.SaveChangesAsync();

        Console.WriteLine("Registering lyckades!\n");
        await Task.Delay(1000);

    }
}
