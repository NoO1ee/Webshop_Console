namespace Webshop_Console.Models;

public class UnitModel : IDModel
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;

    public ICollection<ArticleModel> Articles { get; set; } = new List<ArticleModel>();
}
