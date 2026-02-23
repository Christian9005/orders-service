using Microsoft.EntityFrameworkCore.Storage;
using OrdersInfrastructure.Context;
using OrdersInfrastructure.Interfaces;
using OrdersInfrastructure.Repositories;

namespace OrdersInfrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderContext context;
    private IOrderRepository orderRepository;
    private IAuditLogRepository auditLogRepository;
    private IDbContextTransaction transaction;

    public UnitOfWork(OrderContext context)
    {
        this.context = context;
    }
    public IOrderRepository Orders =>
        this.orderRepository ??= new OrderRepository(context);

    public IAuditLogRepository AuditLogs =>
        this.auditLogRepository ??= new AuditLogRepository(context);

    public OrderContext Context => context;

    public async Task<bool> BeginTransactionAsync()
    {
        this.transaction = await context.Database.BeginTransactionAsync();
        return true;
    }

    public async Task<bool> CommitAsync()
    {
        try
        {
            await context.SaveChangesAsync();
            await transaction?.CommitAsync();
            return true;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    public void Dispose()
    {
        transaction?.Dispose();
        context.Dispose();
    }

    public async Task<bool> RollbackAsync()
    {
        try
        {
            await transaction?.RollbackAsync();
            return true;
        }
        finally
        {
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
