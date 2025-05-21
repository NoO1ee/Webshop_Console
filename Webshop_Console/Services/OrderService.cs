using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class OrderService
{
    readonly MyDbContext _db;
    public OrderService(MyDbContext db) => _db = db;

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _db.Orders.Include(o => o.User).Include(o => o.Items).ThenInclude(i => i.Article).Include(o => o.Payment).ThenInclude(p => p!.Method).OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        return await _db.Orders.Where(o => o.UserId == userId).Include(o => o.Items).ThenInclude(i => i.Article).Include(o => o.Payment).ThenInclude(p => p!.Method).OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public async Task<bool> UpdateOrderAsync(Order order)
    {
        _db.Orders.Update(order);
        var changes = await _db.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _db.Orders.Include(o => o.User).Include(o => o.Items).ThenInclude(i => i.Article).Include(o => o.Payment).ThenInclude(p => p!.Method).FirstOrDefaultAsync(o => o.Id == orderId);
    }
}
