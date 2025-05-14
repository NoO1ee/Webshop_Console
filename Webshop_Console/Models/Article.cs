namespace Webshop_Console.Models;

public class Article : BaseEntity
{
    public string? Name { get; set; }
    public string EanCode { get; set; } = string.Empty;
    public string ArticleCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Storage { get; set; } = 0;
    public string? Bio { get; set; }
    public decimal Price { get; set; }
    public bool IsFeatured { get; set; } = false;

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public int? DiscountId { get; set; }
    public Discount? Discount { get; set; } = null!;

}
