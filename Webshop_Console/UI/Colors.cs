using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.UI;

public class Color
{
    #region STRING COLORS.
    //Röd
    public static string R(string t)
    {
        return $"\x1b[31m{t}\x1b[39m";
    }
    //Grön
    public static string G(string t)
    {
        return $"\x1b[32m{t}\x1b[39m";
    }
    //Gul
    public static string Y(string t)
    {
        return $"\x1b[33m{t}\x1b[39m";
    }
    //Blue
    public static string B(string t)
    {
        return $"\x1b[34m{t}\x1b[39m";
    }
    //Magenta
    public static string M(string t)
    {
        return $"\x1b[35m{t}\x1b[39m";
    }
    //Turkous
    public static string C(string t)
    {
        return $"\x1b[36m{t}\x1b[39m";
    }
    //White
    public static string W(string t)
    {
        return $"\x1b[37m{t}\x1b[39m";
    }

    public static string ERROR(string t)
    {
        return $"\x1b[31m{t.ToUpper()}\x1b[39m";
    }
    #endregion
}

public class CON_Settings
{
    //Cursor visible
    public static void Cursor(int t, bool hide, string msg, bool clear)
    {
        t = t * 1000;
        if (clear)
        {
            Console.Clear();
            Console.CursorVisible = hide;
            Console.WriteLine(msg);
            Thread.Sleep(t);
            Console.CursorVisible = true;
            Console.Clear();
        }
        else
        {
            Console.CursorVisible = hide;
            Console.WriteLine(msg);
            Thread.Sleep(t);
            Console.CursorVisible = true;

        }
    }
}
