using OrdersDomain.Entities;

namespace OrdersApplication.DTOs;

public class CreateOrderRequest
{
    public int ClienteId { get; set; }
    public string Usuario { get; set; }
    public List<OrderDetailRequest> Items { get; set; } = new List<OrderDetailRequest>();
}

public class OrderDetailRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
}