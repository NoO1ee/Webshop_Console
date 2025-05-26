namespace Webshop_Console.Models;

public class OrderItem : IDModel
{
    public int OrderId { get; set; }
    public OrderModel Order { get; set; }
    public int ArticleId { get; set; }
    public ArticleModel Article { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal PriceAtPurchase { get; set; }

}
