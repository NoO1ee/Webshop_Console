using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class OrderHandler
{
    readonly MyDbContext _db;
    public OrderHandler(MyDbContext db) => _db = db;

    public async Task<List<OrderModel>> GetAllOrdersAsync()
    {
        return await _db.Orders.Include(o => o.User).Include(o => o.Items).ThenInclude(i => i.Article).Include(o => o.Payment).ThenInclude(p => p!.Method).OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public async Task<List<OrderModel>> GetOrdersByUserIdAsync(int userId)
    {
        return await _db.Orders.Where(o => o.UserId == userId).Include(o => o.Items).ThenInclude(i => i.Article).Include(o => o.Payment).ThenInclude(p => p!.Method).OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public async Task<bool> UpdateOrderAsync(OrderModel order)
    {
        _db.Orders.Update(order);
        var changes = await _db.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<OrderModel?> GetByIdAsync(int orderId)
    {
        return await _db.Orders.Include(o => o.User).Include(o => o.Items).ThenInclude(i => i.Article).Include(o => o.Payment).ThenInclude(p => p!.Method).FirstOrDefaultAsync(o => o.Id == orderId);
    }
}
