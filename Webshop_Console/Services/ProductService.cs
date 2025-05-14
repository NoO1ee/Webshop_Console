using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class ProductService
{
    readonly MyDbContext _db;
    public ProductService(MyDbContext db) => _db = db;

    public async Task<List<Article>> GetAllAsync()
    {
        return await _db.Articles.Include(a => a.Supplier).Include(a => a.Unit).ToListAsync();
    }

    public async Task<Article?> GetByIdAsync(int id)
    {
        return await _db.Articles.Include(a => a.Supplier).Include(a => a.Unit).FirstOrDefaultAsync(a => a.Id == id);

    }

    public async Task AddAsync(Article article)
    {
        _db.Articles.Add(article);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Article article)
    {
        var existing = await _db.Articles.FindAsync(article.Id);
        if(existing == null)
            return false;

        existing.Name = article.Name;
        existing.Bio = article.Bio;
        existing.Price = article.Price;
        existing.SupplierId = article.SupplierId;
        existing.UnitId = article.UnitId;

        _db.Articles.Update(existing);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Articles.FindAsync(id);
        if(existing == null) return false;

        _db.Articles.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
    
}
