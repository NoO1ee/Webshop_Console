namespace Webshop_Console.Models;

public class AuthorityModel : IDModel
{
    public string? Name { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsOwner { get; set; } = false;
    public ICollection<UserModel> Users { get; set; } = new List<UserModel>();

}
