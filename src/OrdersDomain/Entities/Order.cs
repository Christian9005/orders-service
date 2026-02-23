using OrdersDomain.Enums;
using OrdersDomain.Exceptions;

namespace OrdersDomain.Entities;

public class Order
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public DateTime Date { get; private set; }
    public decimal Total { get; private set; }
    public string User { get; private set; }
    public OrderStatus Status { get; private set; }
    public Guid RequestId { get; private set; }
    public string IdempotencyKey { get; private set; }

    public ICollection<OrderDetail> Details { get; private set; } = new List<OrderDetail>();
    private Order() { }

    public static Order Create(int clientId, string user, Guid requestId, string idempotencyKey)
    {
        if (clientId <= 0)
        {
            throw new ArgumentException("ClientId debe ser mayor a 0", nameof(clientId));
        }

        if (string.IsNullOrWhiteSpace(user))
        {
            throw new ArgumentException("Usuario es requerido", nameof(user));
        }

        return new Order
        {
            ClientId = clientId,
            Date = DateTime.UtcNow,
            Total = 0,
            User = user,
            Status = OrderStatus.Pending,
            RequestId = requestId,
            IdempotencyKey = idempotencyKey
        };
    }

    public void AddDetail(int productId, int quantity, decimal price)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Cantidad debe ser mayor a 0", nameof(quantity));
        }

        if (price < 0)
        {
            throw new ArgumentException("Precio no puede ser negativo", nameof(price));
        }

        var detail = OrderDetail.Create(productId, quantity, price);
        Details.Add(detail);

        RecalculateTotal();
    }

    public void Validate()
    {
        if (!Details.Any())
        {
            throw new DomainException("La orden debe tener al menos un detalle");
        }
        if (Total <= 0)
        {
            throw new DomainException("El total de la orden debe ser mayor a 0");
        }
    }

    private void RecalculateTotal()
    {
        Total = Details.Sum(d => d.Subtotal);
    }

    public void MarkAsProcessed()
    {
        Status = OrderStatus.Processed;
    }

    public void MarkAsCompleted()
    {
            Status = OrderStatus.Completed;
    }

    public void MarkAsFailed(string reason)
    {
        Status = OrderStatus.Failed;
    }
}
