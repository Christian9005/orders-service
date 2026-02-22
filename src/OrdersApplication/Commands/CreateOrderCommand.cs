using MediatR;
using OrdersApplication.DTOs;

namespace OrdersApplication.Commands;

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public int ClienteId { get; set; }
    public string Usuario { get; set; }
    public List<OrderDetailRequest> Items { get; set; }
    public Guid RequestId { get; set; }
    public string IdempotencyKey { get; set; }

    public CreateOrderCommand()
    {
        RequestId = Guid.NewGuid();
        IdempotencyKey = Guid.NewGuid().ToString();
    }
}
