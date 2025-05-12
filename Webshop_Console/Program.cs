using System.Threading.Channels;
using Visual;
using Webshop_Console.Services;
using Webshop_Console.UI;

namespace Webshop_Console;

internal class Program
{
    static async Task Main(string[] args)
    {

        //Console.Title = "Duck4Hire";

        //För windows 10
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        //Menu mainMenu = new Menu("Log in/Register");
        //mainMenu.AddOption("Log in", () => Console.Write("Test"));
        //mainMenu.AddOption("Register", () => Console.Write("Test"));
        //mainMenu.AddOption("Exit", () => mainMenu.Close());
        //mainMenu.SetColors(ConsoleColor.DarkYellow, ConsoleColor.White, ConsoleColor.Red);

        //mainMenu.Display();



        await Menu.ShowMenu("Duck4Hire", "Log in/Register", new (string, Action)[]
        {
            //("Log in", () => Login.RunLogin()),
            ("Register", () => Console.WriteLine("Register test")),
            ("Exit", () => Environment.Exit(0))
        });

    }

    


}
