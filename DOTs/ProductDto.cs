using System.ComponentModel.DataAnnotations;

namespace EasyShop.API.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}