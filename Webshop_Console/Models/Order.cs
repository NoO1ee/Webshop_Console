using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public Payment? Payment { get; set; }

}
