namespace Webshop_Console.Models;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ArticleId { get; set; }
    public Article? Article { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }

}
