using EasyShop.API.DTOs;

namespace EasyShop.API.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();

    Task<ProductDto?> GetByIdAsync(int id);

    Task<ProductSearchResponseDto> SearchAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? sortOrder,
        int page,
        int pageSize);

    Task<ProductDto> CreateAsync(ProductDto productDto);

    Task<ProductDto?> UpdateAsync(
        int id,
        ProductDto productDto);

    Task<bool> DeleteAsync(int id);
}