using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop_Console.Models;

namespace Webshop_Console.Services;

public class ProductHandler
{
    readonly MyDbContext _db;
    public ProductHandler(MyDbContext db) => _db = db;

    public async Task<List<ArticleModel>> GetAllAsync()
    {
        return await _db.Articles.Include(a => a.Supplier).Include(a => a.Unit).Include(a => a.Category).ToListAsync();
    }

    public async Task<List<ArticleModel>> GetByCategoryIdAsync(int category)
    {
        return await _db.Articles.Include(a => a.Supplier).Include(a => a.Unit).Include(a => a.Category).Where(a => a.CategoryId == category).ToListAsync();
    }

    public async Task<ArticleModel?> GetByIdAsync(int id)
    {
        return await _db.Articles.Include(a => a.Supplier).Include(a => a.Unit).Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id);

    }

    public async Task<List<CategoryModel>> GetAllCategoriesAsyc()
    {
        return await _db.Set<CategoryModel>().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task AddAsync(ArticleModel article)
    {
        _db.Articles.Add(article);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(ArticleModel article)
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

    public async Task<Dictionary<int, int>> GetSalesBuProductAsync()
    {
        return await _db.OrderItems.GroupBy(oi => oi.ArticleId).Select(g => new
        {
            ArticleID = g.Key,
            TotalSold = g.Sum(oi => oi.Quantity)
        }
        ).ToDictionaryAsync(x => x.ArticleID, x => x.TotalSold);
    }

    public async Task<List<CategoryModel>> GetAllCategoriesAsync() => await _db.Set<CategoryModel>().OrderBy(c => c.Name).ToListAsync();

    public async Task<CategoryModel> CreateCategoryASync(string name)
    {
        var category = new CategoryModel { Name = name };
        await _db.AddAsync(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<bool> UpdateCategoryAsync(int id, string name)
    {
        var category = await _db.Set<CategoryModel>().FindAsync(id);
        if(category == null) 
            return false;

        category.Name = name;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _db.Set<CategoryModel>().FindAsync(id);
        if(category == null) 
            return false;
        _db.Remove(category);
        await _db.SaveChangesAsync();
        return true;
    }

}
