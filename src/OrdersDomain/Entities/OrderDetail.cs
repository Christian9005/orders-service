namespace OrdersDomain.Entities;

public class OrderDetail
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Subtotal => Quantity * Price;
    public Order Order { get; private set; }
    private OrderDetail() { }
    public static OrderDetail Create(int orderId, int productId, int quantity, decimal price)
    {
        if (orderId <= 0)
        {
            throw new ArgumentException("OrderId debe ser mayor a 0", nameof(orderId));
        }
        if (productId <= 0)
        {
            throw new ArgumentException("ProductId debe ser mayor a 0", nameof(productId));
        }
        if (quantity <= 0)
        {
            throw new ArgumentException("Cantidad debe ser mayor a 0", nameof(quantity));
        }
        if (price < 0)
        {
            throw new ArgumentException("Precio no puede ser negativo", nameof(price));
        }

        return new OrderDetail
        {
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            Price = price
        };
    }
}
