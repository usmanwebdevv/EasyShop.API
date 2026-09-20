using System.Security.Claims;
using EasyShop.API.DTOs;
using EasyShop.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyShop.API.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public CartController(
    ICartService cartService,
    IOrderService orderService)
{
    _cartService = cartService;
    _orderService = orderService;
}

    // GET: api/cart
    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> GetCart()
    {
        var cart = await _cartService.GetCartAsync(GetUserId());

        return Ok(cart);
    }

    // POST: api/cart/items
    [HttpPost("items")]
    public async Task<ActionResult<CartResponseDto>> AddItem(
        [FromBody] AddCartItemDto dto)
    {
        var cart = await _cartService.AddItemAsync(
            GetUserId(), dto);

        return Ok(cart);
    }

    // PUT: api/cart/items/2
    [HttpPut("items/{productId:int}")]
    public async Task<ActionResult<CartResponseDto>> UpdateItem(
        int productId,
        [FromBody] UpdateCartItemDto dto)
    {
        var cart = await _cartService.UpdateItemAsync(
            GetUserId(), productId, dto);

        return Ok(cart);
    }

    // DELETE: api/cart/items/2
    [HttpDelete("items/{productId:int}")]
    public async Task<ActionResult<CartResponseDto>> RemoveItem(
        int productId)
    {
        var cart = await _cartService.RemoveItemAsync(
            GetUserId(), productId);

        return Ok(cart);
    }


    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null ||
            !int.TryParse(claim.Value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user token.");
        }

        return userId;
    }

    [HttpPost("checkout")]
public async Task<ActionResult<OrderResponseDto>> Checkout()
{
    var order = await _orderService.CheckoutAsync(GetUserId());

    return Ok(order);
}
}