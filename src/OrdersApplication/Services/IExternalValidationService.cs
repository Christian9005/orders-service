namespace OrdersApplication.Services;

public interface IExternalValidationService
{
    Task<bool> ValidateClientAsync(int clientId, CancellationToken cancellationToken = default);
}
