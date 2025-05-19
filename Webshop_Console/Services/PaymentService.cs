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
}
