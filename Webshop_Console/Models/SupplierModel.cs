namespace Webshop_Console.Models;

public class SupplierModel : IDModel
{
    public string Name { get; set; } = string.Empty;
    public ICollection<ArticleModel> Articles { get; set; } = new List<ArticleModel>();
}
