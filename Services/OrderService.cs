using EasyShop.API.DTOs;
using EasyShop.API.Models;
using EasyShop.API.Repositories;

namespace EasyShop.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _cartRepository = cartRepository;
    }

    // CREATE ORDER
    public async Task<OrderResponseDto> CreateAsync(
        int userId,
        CreateOrderDto orderDto)
    {
        await using var transaction =
            await _orderRepository.BeginTransactionAsync();

        try
        {
            var order = await CreateOrderCoreAsync(userId, orderDto);

            await transaction.CommitAsync();

            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // CHECKOUT CART
    public async Task<OrderResponseDto> CheckoutAsync(int userId)
    {
        await using var transaction =
            await _orderRepository.BeginTransactionAsync();

        try
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException(
                    "Cart is empty.");
            }

            var orderDto = new CreateOrderDto
            {
                Items = cart.Items.Select(item =>
                    new CreateOrderItemDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
            };

            // Reuse order creation logic without
            // starting or committing another transaction.
            var order = await CreateOrderCoreAsync(
                userId, orderDto);

            // Clear cart inside the SAME transaction.
            await _cartRepository.ClearCartAsync(userId);

            await transaction.CommitAsync();

            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // SHARED ORDER CREATION LOGIC
    private async Task<OrderResponseDto> CreateOrderCoreAsync(
        int userId,
        CreateOrderDto orderDto)
    {
        if (orderDto == null ||
            orderDto.Items == null ||
            !orderDto.Items.Any())
        {
            throw new InvalidOperationException(
                "Order must contain at least one item.");
        }

        if (orderDto.Items.Any(item =>
            item.ProductId <= 0 || item.Quantity <= 0))
        {
            throw new InvalidOperationException(
                "Product ID and quantity must be greater than zero.");
        }

        if (orderDto.Items
            .GroupBy(item => item.ProductId)
            .Any(group => group.Count() > 1))
        {
            throw new InvalidOperationException(
                "Duplicate products are not allowed in an order.");
        }

        var order = new Order
        {
            UserId = userId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        decimal totalAmount = 0;

        foreach (var itemDto in orderDto.Items)
        {
            var product = await _productRepository
                .GetByIdAsync(itemDto.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException(
                    $"Product with ID {itemDto.ProductId} not found.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for {product.Name}.");
            }

            var subtotal = product.Price * itemDto.Quantity;

            totalAmount += subtotal;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });

            product.StockQuantity -= itemDto.Quantity;

            await _productRepository.UpdateAsync(product);
        }

        order.TotalAmount = totalAmount;

        await _orderRepository.AddAsync(order);

        var createdOrder =
            await _orderRepository.GetByIdAsync(order.Id);

        if (createdOrder == null)
        {
            throw new InvalidOperationException(
                "Order could not be created.");
        }

        return MapToDto(createdOrder);
    }

    // MY ORDERS
    public async Task<IEnumerable<OrderResponseDto>>
        GetMyOrdersAsync(int userId)
    {
        var orders =
            await _orderRepository.GetByUserIdAsync(userId);

        return orders.Select(MapToDto);
    }

    // ALL ORDERS
    public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders.Select(MapToDto);
    }

    // GET ORDER BY ID
    public async Task<OrderResponseDto?> GetByIdAsync(
        int orderId,
        int userId,
        bool isAdmin)
    {
        var order =
            await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return null;

        if (!isAdmin && order.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access this order.");
        }

        return MapToDto(order);
    }

    // UPDATE ORDER STATUS
    public async Task<OrderResponseDto?> UpdateStatusAsync(
        int orderId,
        string status)
    {
        var order =
            await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return null;

        if (string.IsNullOrWhiteSpace(status))
        {
            throw new InvalidOperationException(
                "Status is required.");
        }

        var allowedTransitions =
            new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["Pending"] = "Processing",
                ["Processing"] = "Shipped",
                ["Shipped"] = "Delivered"
            };

        if (!allowedTransitions.TryGetValue(
            order.Status, out var nextStatus))
        {
            throw new InvalidOperationException(
                $"Cannot change status from {order.Status}.");
        }

        if (!nextStatus.Equals(
            status.Trim(),
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Order must move from {order.Status} to {nextStatus}.");
        }

        order.Status = nextStatus;

        await _orderRepository.UpdateAsync(order);

        return MapToDto(order);
    }

    // CANCEL ORDER
    public async Task<OrderResponseDto?> CancelOrderAsync(
        int orderId)
    {
        await using var transaction =
            await _orderRepository.BeginTransactionAsync();

        try
        {
            var order =
                await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return null;

            if (order.Status == "Cancelled")
            {
                throw new InvalidOperationException(
                    "Order is already cancelled.");
            }

            if (order.Status != "Pending" &&
                order.Status != "Processing")
            {
                throw new InvalidOperationException(
                    "This order cannot be cancelled.");
            }

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository
                    .GetByIdAsync(item.ProductId);

                if (product == null)
                {
                    throw new KeyNotFoundException(
                        $"Product {item.ProductId} not found.");
                }

                product.StockQuantity += item.Quantity;

                await _productRepository.UpdateAsync(product);
            }

            order.Status = "Cancelled";

            await _orderRepository.UpdateAsync(order);

            await transaction.CommitAsync();

            return MapToDto(order);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // MAP ORDER TO DTO
    private static OrderResponseDto MapToDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,

            Items = order.OrderItems
                .Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName =
                        oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.UnitPrice * oi.Quantity
                })
                .ToList()
        };
    }
}