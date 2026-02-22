using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersInfrastructure.Context;
using OrdersInfrastructure.Interfaces;
using OrdersInfrastructure.Persistence;
using OrdersInfrastructure.Repositories;
using OrdersInfrastructure.Security;

namespace OrdersInfrastructure.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var jwtSettings = new JwtSettings
        {
            SecretKey = configuration["JwtSettings:SecretKey"]!,
            Issuer = configuration["JwtSettings:Issuer"]!,
            Audience = configuration["JwtSettings:Audience"]!,
            ExpirationMinutes = int.Parse(configuration["JwtSettings:ExpirationMinutes"] ?? "60")
        };
        services.AddSingleton(jwtSettings);
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

        return services;
    }
}
