using OrdersInfrastructure.Interfaces;

namespace OrdersInfrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync();
    Task<bool> BeginTransactionAsync();
    Task<bool> CommitAsync();
    Task<bool> RollbackAsync();
}
