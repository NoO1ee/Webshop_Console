using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.UI;

public class MenuOption
{
    public string Text { get; set; }
    public Action Action { get; set; }

    public MenuOption(string text, Action action)
    {
        Text = text;
        Action = action;
    }
}
