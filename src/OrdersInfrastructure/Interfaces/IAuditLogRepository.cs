using OrdersDomain.Entities;

namespace OrdersInfrastructure.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<IEnumerable<AuditLog>> GetByRequestIdAsync(Guid requestId);
    Task<IEnumerable<AuditLog>> GetBytOrderIdAsync(int orderId);
}
