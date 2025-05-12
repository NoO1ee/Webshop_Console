using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ArticleId { get; set; }
    public Article Article { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }

}
