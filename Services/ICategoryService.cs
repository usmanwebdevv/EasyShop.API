using EasyShop.API.DTOs;

namespace EasyShop.API.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<CategoryDto> CreateAsync(CategoryDto categoryDto);

    Task<CategoryDto?> UpdateAsync(int id, CategoryDto categoryDto);

    Task<bool> DeleteAsync(int id);
}