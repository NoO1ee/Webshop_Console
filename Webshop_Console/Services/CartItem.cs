using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class CartItem
{
    public ArticleModel Article { get; set; }
    public int Quantity { get; set; }
}
