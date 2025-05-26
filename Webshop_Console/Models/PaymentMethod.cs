namespace Webshop_Console.Models;

public class PaymentMethod : IDModel
{
    public string Name { get; set; }
    public ICollection<PaymentModel> Payments { get; set; } = new List<PaymentModel>();
}
