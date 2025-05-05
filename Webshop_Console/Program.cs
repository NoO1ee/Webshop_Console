using System.Threading.Channels;
using Visual;

namespace Webshop_Console;

internal class Program
{
    static void Main(string[] args)
    {

        Console.Title = "Duck4Hire";

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Menu mainMenu = new Menu("Log in/Register");
        mainMenu.AddOption("Log in", () => Console.Write("Test"));
        mainMenu.AddOption("Register", () => Console.Write("Test"));
        mainMenu.AddOption("Exit", () => mainMenu.Close());
        mainMenu.SetColors(ConsoleColor.DarkYellow, ConsoleColor.White, ConsoleColor.Red);

        mainMenu.Display();
    }


}
