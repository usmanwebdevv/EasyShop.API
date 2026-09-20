using EasyShop.API.DTOs;

namespace EasyShop.API.Services;

public interface ICartService
{
    Task<CartResponseDto> GetCartAsync(int userId);

    Task<CartResponseDto> AddItemAsync(
        int userId,
        AddCartItemDto dto);

    Task<CartResponseDto> UpdateItemAsync(
        int userId,
        int productId,
        UpdateCartItemDto dto);

    Task<CartResponseDto> RemoveItemAsync(
        int userId,
        int productId);
}