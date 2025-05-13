namespace Webshop_Console.Models;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
