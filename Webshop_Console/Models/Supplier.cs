namespace Webshop_Console.Models;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
