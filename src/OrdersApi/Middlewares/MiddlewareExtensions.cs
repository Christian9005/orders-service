namespace OrdersApi.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestIdMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestIdMiddleware>();
    }

    public static IApplicationBuilder UseIdempotencyMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<IdempotencyMiddleware>();
    }

    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
