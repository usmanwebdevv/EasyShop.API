using EasyShop.API.Models;

namespace EasyShop.API.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    
    Task ClearCartAsync(int userId);


    Task<Cart> CreateAsync(Cart cart);
    

    Task SaveChangesAsync();
}