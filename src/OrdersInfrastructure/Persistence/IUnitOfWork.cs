using OrdersInfrastructure.Context;
using OrdersInfrastructure.Interfaces;

namespace OrdersInfrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    IAuditLogRepository AuditLogs { get; }
    OrderContext Context { get; }

    Task<int> SaveChangesAsync();
    Task<bool> BeginTransactionAsync();
    Task<bool> CommitAsync();
    Task<bool> RollbackAsync();
}
