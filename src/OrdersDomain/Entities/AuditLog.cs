using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersDomain.Entities;

public class AuditLog
{
    public int Id { get; private set; }
    public int? OrderId { get; private set; }
    public Guid RequestId { get; private set; }
    public string User { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public string Occurence { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Level { get; private set; } = string.Empty;
    private AuditLog() { }
    public static AuditLog Create(int? orderId, Guid requestId, string user, string occurence, string description, string level = "Info")
    {
        return new AuditLog
        {
            OrderId = orderId,
            RequestId = requestId,
            User = user,
            Date = DateTime.UtcNow,
            Occurence = occurence,
            Description = description,
            Level = level
        };
    }
}
