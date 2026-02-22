namespace OrdersApplication.DTOs;

public class TokenRequest
{
    public int ClientId { get; set; }
    public string User { get; set; } = string.Empty;
}
