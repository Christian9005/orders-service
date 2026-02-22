using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersDomain.Exceptions;

public class DomainException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object> Details { get; }

    public DomainException(string message, string errorCode = "DOMAIN_ERROR", Dictionary<string, object>? details = null) : base(message)
    {
        ErrorCode = errorCode;
        Details = details ?? new Dictionary<string, object>();
    }
}
