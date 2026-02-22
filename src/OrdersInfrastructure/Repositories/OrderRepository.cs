using Microsoft.EntityFrameworkCore;
using OrdersDomain.Entities;
using OrdersInfrastructure.Context;
using OrdersInfrastructure.Interfaces;

namespace OrdersInfrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderContext context;

    public OrderRepository(OrderContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);
    }

    public async Task DeleteAsync(int id)
    {
        var order = await GetByIdAsync(id);
        if (order != null)
        {
            context.Orders.Remove(order);
        }
    }

    public async Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await context.Orders
            .AnyAsync(o => o.IdempotencyKey == idempotencyKey);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await context.Orders
            .Include(o => o.Details)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        return await context.Orders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> GetByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await context.Orders
            .FirstOrDefaultAsync(o => o.IdempotencyKey == idempotencyKey);
    }

    public async Task<Order> GetByRequestIdAsync(Guid requestId)
    {
        return await context.Orders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.RequestId == requestId);
    }

    public async Task UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await Task.CompletedTask;
    }
}
