using EasyShop.API.DTOs;
using EasyShop.API.Models;
using EasyShop.API.Repositories;

namespace EasyShop.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return null;
        }

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto categoryDto)
    {
        // Remove unnecessary spaces from category name
        var categoryName = categoryDto.Name.Trim();

        // Check whether category already exists
        var exists = await _categoryRepository
            .ExistsByNameAsync(categoryName);

        if (exists)
        {
            throw new InvalidOperationException(
                "Category already exists."
            );
        }

        var category = new Category
        {
            Name = categoryName,
            Description = categoryDto.Description.Trim()
        };

        await _categoryRepository.AddAsync(category);

        return await GetByIdAsync(category.Id)
            ?? throw new Exception("Category could not be created.");
    }

    public async Task<CategoryDto?> UpdateAsync(
        int id,
        CategoryDto categoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return null;
        }

        var categoryName = categoryDto.Name.Trim();

        category.Name = categoryName;
        category.Description = categoryDto.Description.Trim();

        await _categoryRepository.UpdateAsync(category);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return false;
        }

        await _categoryRepository.DeleteAsync(category);

        return true;
    }
}