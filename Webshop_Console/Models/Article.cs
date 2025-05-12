using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.Models;

public class Article
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string EanCode { get; set; } = string.Empty;
    public string ArticleCode { get; set; } = string.Empty;
    public int Storage { get; set; } = 0;
    public decimal Price { get; set; }

    public int SuplierId { get; set; }
    public Supplier Suplier { get; set; } = null!;

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public int DiscountId { get; set; }
    public Discount Discount { get; set; } = null!;

}
