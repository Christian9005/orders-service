using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersDomain.ValueObjects;

public class RequestId
{
    public Guid Value { get; }
    private RequestId(Guid value = default) 
    {
        Value = value == default ? Guid.NewGuid() : value;
    }
    
    public static implicit operator Guid(RequestId requestId) => requestId.Value;
    public static implicit operator RequestId(Guid value) => new (value);

    public override string ToString() => Value.ToString();
}
