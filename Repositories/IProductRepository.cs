using EasyShop.API.Models;

namespace EasyShop.API.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<bool> ExistsByNameAsync(string name);

    Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? sortOrder,
        int page,
        int pageSize);

    Task<Product> AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task DeleteAsync(Product product);
}