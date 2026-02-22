using OrdersInfrastructure.Security;

namespace OrdersApi.Middlewares;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<JwtAuthenticationMiddleware> logger;

    public JwtAuthenticationMiddleware(RequestDelegate next, ILogger<JwtAuthenticationMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IJwtTokenProvider tokenProvider)
    {
        var token = ExtractToken(context);

        if (!string.IsNullOrEmpty(token))
        {
            var principal = tokenProvider.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
                logger.LogInformation("JWT token validated successfully.");
            }
        }

        await next(context);
    }

    private string ExtractToken(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader)) return null;

        const string prefix = "Bearer ";
        if (authHeader.StartsWith(prefix))
        {
            return authHeader.Substring(prefix.Length);
        }

        return null;
    }
}
