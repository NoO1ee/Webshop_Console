using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.Models;

public class Discount
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Percentage { get; set; } = 0.0;
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
