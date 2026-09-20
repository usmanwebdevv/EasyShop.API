using EasyShop.API.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace EasyShop.API.Repositories;

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order);

    Task<Order?> GetByIdAsync(int id);

    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);

    Task<IEnumerable<Order>> GetAllAsync();

    Task UpdateAsync(Order order);

    Task<IDbContextTransaction> BeginTransactionAsync();
}