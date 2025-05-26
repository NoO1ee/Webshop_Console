using Webshop_Console.Models;
using Webshop_Console.Services;
using Webshop_Console.UI;
using System.Text;
using System.Linq.Expressions;

namespace Webshop_Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        //För windows 10
        Console.OutputEncoding = Encoding.UTF8;

        using var db = new MyDbContext();
        var auth = new LoginHandler(db);
        var prod = new ProductHandler(db);
        var menus = new MenuManager(auth, db, prod);
        await menus.ShowMainMenuAsync();
    }

}
