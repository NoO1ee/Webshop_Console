using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class CartService
{
    readonly MyDbContext _db;
    readonly List<CartItem> _items = new();

    public CartService(MyDbContext db) => _db = db;

    public async Task AddToCart(Article article, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.Article.Id == article.Id);
        if(existing != null)
            existing.Quantity += quantity;
        else if(article.Storage <= 0)
        {
            Console.WriteLine($"Varan är tyvärr slut.");
            await Task.Delay(1000);
        }
        else
        {
            Console.WriteLine($"\n{article.Name} har lagts till i kundvagnen");
            _items.Add(new CartItem { Article = article, Quantity = quantity });
            article.Storage--;
            await Task.Delay(1000);
        }
    }

    public void RemoveFromCart(int articleId, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.Article.Id == articleId);
        if (existing == null)
            return;
        existing.Quantity -= quantity;
        if(existing.Quantity <= 0)
            _items.Remove(existing);
    }

    public IReadOnlyList<CartItem> GetItems() => _items.AsReadOnly();

    public async Task<Order?> CheckoutAsync(User user)
    {
        if(!_items.Any())
            return null;

        var order = new Order
        {
            UserId = user.Id,
            OrderDate = DateTime.Now,
            Items = _items.Select(i => new OrderItem 
            { 
                ArticleId = i.Article.Id, 
                Quantity = i.Quantity, 
                PriceAtPurchase = i.Article.Price * i.Quantity,
                UnitPrice = i.Article.Price
            }).ToList(),
            TotalAmount = _items.Sum(i => i.Article.Price * i.Quantity)
        };

        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
        _items.Clear();
        return order;
    }
}
