using System.Threading.Channels;

namespace Webshop_Console;

internal class Program
{
    static void Main(string[] args)
    {

        Console.Title = "Test";

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Menu mainMenu = new Menu("MAIN MENU");
        mainMenu.AddOption("Products", () => Console.Write("Test"));

        mainMenu.Display();
    }


}
