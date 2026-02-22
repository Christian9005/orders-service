using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace OrdersApplication.Services;

public class ExternalValidationService : IExternalValidationService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<ExternalValidationService> logger;
    private readonly IAsyncPolicy<HttpResponseMessage> resiliencePolicy;

    public ExternalValidationService(HttpClient httpClient, ILogger<ExternalValidationService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.resiliencePolicy = CreateResiliencePolicy();
    }

    private IAsyncPolicy<HttpResponseMessage>? CreateResiliencePolicy()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .OrResult<HttpResponseMessage>(r =>
                (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, attempt, context) =>
                {
                    logger.LogWarning(
                        "Retry attempt {AttemptNumber} for external validation after {DelaySeconds}s",
                        attempt, timespan.TotalSeconds);
                });

        var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .OrResult<HttpResponseMessage>(r =>
                    (int)r.StatusCode >= 500)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, duration, context) =>
                    {
                        logger.LogError(
                            "Circuit breaker opened for external validation. Duration: {Duration}s",
                            duration.TotalSeconds);
                    },
                    onReset: (context) =>
                    {
                        logger.LogInformation("Circuit breaker reset for external validation");
                    });

        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(5));

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
    }

    public async Task<bool> ValidateClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Validating client {ClienteId} with external service", clientId);

            // Usar el endpoint de JSONPlaceholder
            var url = $"https://jsonplaceholder.typicode.com/users/{clientId}";

            var response = await resiliencePolicy.ExecuteAsync(
                async () => await httpClient.GetAsync(url, cancellationToken));

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Client {ClienteId} validated successfully", clientId);
                return true;
            }

            logger.LogWarning("External service returned status {StatusCode} for client {ClienteId}",
                response.StatusCode, clientId);

            return false;
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogError(ex, "Circuit breaker is open for client validation");
            // En caso de circuit breaker abierto, retornar false (rechazar pedido)
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating client {ClienteId}", clientId);
            return false;
        }
    }
}
