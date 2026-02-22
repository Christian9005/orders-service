using Microsoft.Extensions.DependencyInjection;
using OrdersApplication.Services;

namespace OrdersApplication.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtension).Assembly));

        services.AddScoped<IExternalValidationService, ExternalValidationService>();
        
        return services;
    }
}
