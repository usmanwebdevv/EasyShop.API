using EasyShop.API.Data;
using EasyShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyShop.API.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task ClearCartAsync(int userId)
{
    var cart = await _context.Carts
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null)
        return;

    _context.CartItems.RemoveRange(cart.Items);

    await _context.SaveChangesAsync();
}

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> CreateAsync(Cart cart)
    {
        _context.Carts.Add(cart);

        await _context.SaveChangesAsync();

        return cart;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}