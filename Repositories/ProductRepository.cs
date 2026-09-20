using EasyShop.API.Data;
using EasyShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyShop.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Products
            .AnyAsync(p => p.Name.ToLower() == name.ToLower());
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? sortOrder,
        int page,
        int pageSize)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var isDescending =
                string.Equals(
                    sortOrder,
                    "desc",
                    StringComparison.OrdinalIgnoreCase);

            switch (sortBy.ToLower())
            {
                case "price":
                    query = isDescending
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price);
                    break;

                case "name":
                    query = isDescending
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name);
                    break;

                case "stock":
                    query = isDescending
                        ? query.OrderByDescending(p => p.StockQuantity)
                        : query.OrderBy(p => p.StockQuantity);
                    break;

                case "id":
                    query = isDescending
                        ? query.OrderByDescending(p => p.Id)
                        : query.OrderBy(p => p.Id);
                    break;

                default:
                    query = query.OrderByDescending(p => p.Id);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(p => p.Id);
        }

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
    }
}