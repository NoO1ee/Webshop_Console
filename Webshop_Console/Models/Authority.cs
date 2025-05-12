namespace Webshop_Console.Models;

public class Authority : BaseEntity
{
    public string? Name { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsOwner { get; set; } = false;
    public ICollection<User> Users { get; set; } = new List<User>();

}
