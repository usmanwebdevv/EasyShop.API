using EasyShop.API.DTOs;
using EasyShop.API.Models;
using EasyShop.API.Repositories;

namespace EasyShop.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
private readonly ICategoryRepository _categoryRepository;

public ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
{
    _productRepository = productRepository;
    _categoryRepository = categoryRepository;
}


public async Task<ProductSearchResponseDto> SearchAsync(
    string? search,
    int? categoryId,
    decimal? minPrice,
    decimal? maxPrice,
    string? sortBy,
    string? sortOrder,
    int page,
    int pageSize)
{
    if (page < 1)
    {
        page = 1;
    }

    if (pageSize < 1)
    {
        pageSize = 10;
    }

    if (pageSize > 100)
    {
        pageSize = 100;
    }

    if (minPrice.HasValue && minPrice.Value < 0)
    {
        throw new ArgumentException("Minimum price cannot be negative.");
    }

    if (maxPrice.HasValue && maxPrice.Value < 0)
    {
        throw new ArgumentException("Maximum price cannot be negative.");
    }

    if (minPrice.HasValue &&
        maxPrice.HasValue &&
        minPrice.Value > maxPrice.Value)
    {
        throw new ArgumentException(
            "Minimum price cannot be greater than maximum price.");
    }

    var result = await _productRepository.SearchAsync(
        search,
        categoryId,
        minPrice,
        maxPrice,
        sortBy,
        sortOrder,
        page,
        pageSize);

    var products = result.Products;

    var items = products.Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty
    });

    var totalPages = (int)Math.Ceiling(
        result.TotalCount / (double)pageSize);

    return new ProductSearchResponseDto
    {
        Items = items,
        Page = page,
        PageSize = pageSize,
        TotalItems = result.TotalCount,
        TotalPages = totalPages
    };
}
    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty
        });
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty
        };
    }

  
public async Task<ProductDto> CreateAsync(ProductDto productDto)
{
    var category = await _categoryRepository
        .GetByIdAsync(productDto.CategoryId);

    if (category == null)
    {
        throw new KeyNotFoundException("Category not found.");
    }

    var productName = productDto.Name.Trim();

    var exists = await _productRepository
        .ExistsByNameAsync(productName);

    if (exists)
    {
        throw new InvalidOperationException(
            "Product already exists."
        );
    }

    var product = new Product
    {
        Name = productName,
        Description = productDto.Description.Trim(),
        Price = productDto.Price,
        StockQuantity = productDto.StockQuantity,
        CategoryId = productDto.CategoryId,
        CreatedAt = DateTime.UtcNow
    };

    await _productRepository.AddAsync(product);

    return await GetByIdAsync(product.Id)
        ?? throw new Exception("Product could not be created.");
}
   public async Task<ProductDto?> UpdateAsync(
    int id,
    ProductDto productDto)
{
    var product = await _productRepository.GetByIdAsync(id);

    if (product == null)
    {
        return null;
    }

    var category = await _categoryRepository
        .GetByIdAsync(productDto.CategoryId);

    if (category == null)
    {
        throw new KeyNotFoundException("Category not found.");
    }

    var productName = productDto.Name.Trim();

    // Check whether another product already has this name
    var exists = await _productRepository
        .ExistsByNameAsync(productName);

    if (exists && !product.Name.Equals(
        productName,
        StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "Product already exists."
        );
    }

    product.Name = productName;
    product.Description = productDto.Description.Trim();
    product.Price = productDto.Price;
    product.StockQuantity = productDto.StockQuantity;
    product.CategoryId = productDto.CategoryId;

    await _productRepository.UpdateAsync(product);

    return await GetByIdAsync(id);
}
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product);

        return true;
    }
}