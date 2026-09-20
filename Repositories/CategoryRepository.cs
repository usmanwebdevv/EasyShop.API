using EasyShop.API.Data;
using EasyShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyShop.API.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .ToListAsync();
    }
    
    public async Task<bool> ExistsByNameAsync(string name)
{
    return await _context.Categories
        .AnyAsync(c => c.Name.ToLower() == name.ToLower());
}

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
    }
}