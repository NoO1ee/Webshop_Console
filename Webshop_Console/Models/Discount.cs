using System;
using System.Collections.Generic;
using System.Linq;
namespace Webshop_Console.Models;

public class Discount : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
