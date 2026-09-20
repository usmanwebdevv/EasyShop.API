using EasyShop.API.DTOs;

namespace EasyShop.API.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(
        int userId,
        CreateOrderDto orderDto);

    Task<OrderResponseDto> CheckoutAsync(int userId);

    Task<OrderResponseDto?> CancelOrderAsync(int orderId);

    Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int userId);

    Task<IEnumerable<OrderResponseDto>> GetAllAsync();

    Task<OrderResponseDto?> GetByIdAsync(
        int orderId,
        int userId,
        bool isAdmin);

    Task<OrderResponseDto?> UpdateStatusAsync(
        int orderId,
        string status);
}