using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class PaymentService
{

    readonly MyDbContext _db;

    public PaymentService(MyDbContext db) => _db = db;

    public async Task<List<PaymentMethod>> GetAllMethodsAsync()
    {
        return await _db.PaymentMethods.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Payment> CreatePaymentAsync(int orderId, int methodId, decimal amount)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            MethodId = methodId,
            Amount = amount,
            PaymentDate = DateTime.Now
        };
        await _db.Payments.AddAsync(payment);
        await _db.SaveChangesAsync();
        return payment;
    }

    public async Task<List<PaymentMethod>> GetAllPaymentMethodsAsync()
    {
        return await _db.PaymentMethods.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(string name)
    {
        var method = new PaymentMethod { Name = name };
        await _db.PaymentMethods.AddAsync(method);
        await _db.SaveChangesAsync();
        return method;
    }

    public async Task<bool> UpdateMethodAsync(int mehodId, string newName)
    {
        var method = await _db.PaymentMethods.FindAsync(mehodId);
        if(method == null)
            return false;

        method.Name = newName;
        _db.PaymentMethods.Update(method);
        var changes = await _db.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeletePaymentMethodAsync(int methodId)
    {
        var method = await _db.PaymentMethods.FindAsync(methodId);
        if(method == null)
            return false;
        _db.PaymentMethods.Remove(method);
        var changes = await _db.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<Payment> CreatePaymentMethodAsync(int orderId, int methodId, decimal amount)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            MethodId = methodId,
            Amount = amount,
            PaymentDate = DateTime.Now
        };
        await _db.Payments.AddAsync(payment);
        await _db.SaveChangesAsync();
        return payment;
    }
}
