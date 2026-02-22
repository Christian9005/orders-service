using OrdersDomain.Entities;

namespace OrdersApplication.DTOs;

public class OrderResponse
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Usuario { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; }
    public Guid RequestId { get; set; }
    public List<OrderDetailResponse> Detalles { get; set; } = new List<OrderDetailResponse>();

    public static OrderResponse FromEntity(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            ClienteId = order.ClientId,
            Usuario = order.User,
            Fecha = order.Date,
            Total = order.Total,
            Estado = order.Status.ToString(),
            RequestId = order.RequestId,
            Detalles = order.Details.
            Select(d => new OrderDetailResponse
            {
                Id = d.Id,
                ProductoId = d.ProductId,
                Cantidad = d.Quantity,
                Precio = d.Price,
                Subtotal = d.Subtotal
            })
            .ToList()
        };
    }
}

public class OrderDetailResponse
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Subtotal { get; set; }
}