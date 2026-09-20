using EasyShop.API.DTOs;
using EasyShop.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ProductSearchResponseDto>> SearchProducts(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _productService.SearchAsync(
            search,
            categoryId,
            minPrice,
            maxPrice,
            sortBy,
            sortOrder,
            page,
            pageSize);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        ProductDto productDto)
    {
        var product = await _productService.CreateAsync(productDto);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        int id,
        ProductDto productDto)
    {
        var product = await _productService.UpdateAsync(
            id,
            productDto);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound("Product not found");
        }

        return Ok("Product deleted successfully");
    }
}