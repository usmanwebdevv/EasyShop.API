namespace EasyShop.API.DTOs;

public class CartResponseDto
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public List<CartItemResponseDto> Items { get; set; } = new();
}

public class CartItemResponseDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }
}