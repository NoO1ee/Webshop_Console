namespace Webshop_Console.Models;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public int? PaymentId { get; set; }
    public Payment? Payment { get; set; }

}
