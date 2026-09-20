using System.Security.Claims;
using EasyShop.API.DTOs;
using EasyShop.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder(
        CreateOrderDto orderDto)
    {
        var userId = GetUserId();

        var order = await _orderService.CreateAsync(
            userId,
            orderDto);

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = order.Id },
            order);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>>
        GetMyOrders()
    {
        var userId = GetUserId();

        var orders =
            await _orderService.GetMyOrdersAsync(userId);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(
        int id)
    {
        var userId = GetUserId();

        var isAdmin =
            User.IsInRole("Admin");

        var order = await _orderService.GetByIdAsync(
            id,
            userId,
            isAdmin);

        if (order == null)
            return NotFound("Order not found.");

        return Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>>
        GetAllOrders()
    {
        var orders = await _orderService.GetAllAsync();

        return Ok(orders);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderResponseDto>>
        UpdateStatus(
            int id,
            [FromQuery] string status)
    {
        var order =
            await _orderService.UpdateStatusAsync(id, status);

        if (order == null)
            return NotFound("Order not found.");

        return Ok(order);
    }
    

    private int GetUserId()
    {
        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid user token.");
        }

        return int.Parse(userIdClaim.Value);
    }

    [Authorize(Roles = "Admin")]
[HttpPut("{id:int}/cancel")]
public async Task<ActionResult<OrderResponseDto>> CancelOrder(int id)
{
    var order = await _orderService.CancelOrderAsync(id);

    if (order == null)
        return NotFound("Order not found.");

    return Ok(order);
}
}