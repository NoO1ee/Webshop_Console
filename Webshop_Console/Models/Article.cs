using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.Models;

internal class Article
{
    public int ID { get; set; }
    public string EanCode { get; set; } = string.Empty;
    public string ArticleCode { get; set; } = string.Empty;

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

}
