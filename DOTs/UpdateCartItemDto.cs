using System.ComponentModel.DataAnnotations;

namespace EasyShop.API.DTOs;

public class UpdateCartItemDto
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}