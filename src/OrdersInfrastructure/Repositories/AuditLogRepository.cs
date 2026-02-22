using Microsoft.EntityFrameworkCore;
using OrdersDomain.Entities;
using OrdersInfrastructure.Context;
using OrdersInfrastructure.Interfaces;

namespace OrdersInfrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly OrderContext context;

    public AuditLogRepository(OrderContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(AuditLog log)
    {
        await context.AuditLogs.AddAsync(log);
    }

    public async Task<IEnumerable<AuditLog>> GetByRequestIdAsync(Guid requestId)
    {
        return await context.AuditLogs
            .Where(l => l.RequestId == requestId)
            .OrderBy(l => l.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetBytOrderIdAsync(int orderId)
    {
        return await context.AuditLogs
            .Where(l => l.OrderId == orderId)
            .OrderBy(l => l.Date)
            .ToListAsync();
    }
}
