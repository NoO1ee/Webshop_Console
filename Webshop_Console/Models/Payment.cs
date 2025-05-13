namespace Webshop_Console.Models;

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public int MethodId { get; set; }
    public PaymentMethod? Method { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

}
