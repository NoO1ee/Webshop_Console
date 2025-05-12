namespace Webshop_Console.Models;

public class User : BaseEntity
{
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int? Age { get; set; }
    public virtual ICollection<Authority> Authorities { get; set; } = new List<Authority>();
}
