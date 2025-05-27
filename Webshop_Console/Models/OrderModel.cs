namespace Webshop_Console.Models;

public class OrderModel : IDModel
{
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal Net { get; set; }
    public decimal Vat { get; set; }
    public decimal TotalAmount { get; set; }

    public int? UserId { get; set; }
    public UserModel? User { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public int? PaymentId { get; set; }
    public PaymentModel? Payment { get; set; }

}
