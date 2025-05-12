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

    public Task<List<Article>> GetFeaturedAsync(int count = 3) => 
        _db.Articles
            .OrderBy(a => a.ID)
            .Take(count)
            .ToListAsync();

    public Task<List<Article>> GetAllAsync() =>
        _db.Articles
            .OrderBy(a => a.ID)
            .ToListAsync();

    public async Task AddAsync(Article art)
    {
        _db.Articles.Add(art);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Article art)
    {
        _db.Articles.Update(art);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var art = await _db.Articles.FindAsync(id);
        if (art != null)
        {
            _db.Articles.Remove(art);
            await _db.SaveChangesAsync();
        }
    }

    
}
