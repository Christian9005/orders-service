using Microsoft.Extensions.DependencyInjection;
using OrdersApplication.Services;
using FluentValidation;
using OrdersApplication.Behaviors;

namespace OrdersApplication.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationExtension).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ApplicationExtension).Assembly);

        services.AddHttpClient<IExternalValidationService, ExternalValidationService>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

        return services;
    }
}
