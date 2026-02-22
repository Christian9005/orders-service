using OrdersDomain.Entities;

namespace OrdersInfrastructure.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(int id);
    Task<Order> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<Order> GetByRequestIdAsync(Guid requestId);
    Task<IEnumerable<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int id);
    Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey);
}
