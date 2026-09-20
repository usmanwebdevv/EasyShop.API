using EasyShop.API.DTOs;
using EasyShop.API.Models;
using EasyShop.API.Repositories;

namespace EasyShop.API.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    // 1. View Cart
    public async Task<CartResponseDto> GetCartAsync(int userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart == null)
        {
            return new CartResponseDto
            {
                UserId = userId
            };
        }

        return MapToDto(cart);
    }

    // 2. Add Product to Cart
    public async Task<CartResponseDto> AddItemAsync(
        int userId,
        AddCartItemDto dto)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");

        var product = await _productRepository.GetByIdAsync(dto.ProductId);

        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };

            await _cartRepository.CreateAsync(cart);
        }

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ProductId == dto.ProductId);

        int newQuantity;

        try
        {
            newQuantity = checked(
                (existingItem?.Quantity ?? 0) + dto.Quantity);
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException(
                "Quantity is too large.");
        }

        if (newQuantity > product.StockQuantity)
            throw new InvalidOperationException(
                "Not enough stock available.");

        if (existingItem != null)
        {
            existingItem.Quantity = newQuantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = dto.ProductId,
                Product = product,
                Quantity = dto.Quantity
            });
        }

        await _cartRepository.SaveChangesAsync();

        return MapToDto(cart);
    }

    // 3. Update Product Quantity
    public async Task<CartResponseDto> UpdateItemAsync(
        int userId,
        int productId,
        UpdateCartItemDto dto)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");

        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart == null)
            throw new KeyNotFoundException("Cart not found.");

        var item = cart.Items
            .FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            throw new KeyNotFoundException("Cart item not found.");

        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        if (dto.Quantity > product.StockQuantity)
            throw new InvalidOperationException(
                "Not enough stock available.");

        item.Quantity = dto.Quantity;

        await _cartRepository.SaveChangesAsync();

        return MapToDto(cart);
    }

    // 4. Remove Product from Cart
    public async Task<CartResponseDto> RemoveItemAsync(
        int userId,
        int productId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart == null)
            throw new KeyNotFoundException("Cart not found.");

        var item = cart.Items
            .FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            throw new KeyNotFoundException("Cart item not found.");

        cart.Items.Remove(item);

        await _cartRepository.SaveChangesAsync();

        return MapToDto(cart);
    }

    // Convert Cart to Response DTO
    private static CartResponseDto MapToDto(Cart cart)
    {
        var items = cart.Items.Select(i => new CartItemResponseDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? "Unknown Product",
            Price = i.Product?.Price ?? 0,
            Quantity = i.Quantity,
            Subtotal = (i.Product?.Price ?? 0) * i.Quantity
        }).ToList();

        return new CartResponseDto
        {
            CartId = cart.Id,
            UserId = cart.UserId,
            Items = items,
            TotalAmount = items.Sum(i => i.Subtotal)
        };
    }
}