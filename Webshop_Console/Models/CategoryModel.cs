using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop_Console.Models;

public class CategoryModel : IDModel
{
    public string Name { get; set; } = string.Empty;

    public ICollection<ArticleModel> Articles { get; set; } = new List<ArticleModel>();
}
