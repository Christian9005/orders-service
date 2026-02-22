using OrdersDomain.Exceptions;

namespace OrdersApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var requestId = context.Items["RequestId"] as Guid? ?? Guid.NewGuid();
            logger.LogError(ex, "Unhandled exception. RequestId: {RequestId}", requestId);
            await HandleExceptionAsync(context, ex, requestId);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, Guid requestId)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            DomainException domainEx => new
            {
                success = false,
                message = domainEx.Message,
                requestId,
                errorCode = domainEx.ErrorCode,
                statusCode = StatusCodes.Status400BadRequest
            },
            ArgumentException argEx => new
            {
                success = false,
                message = argEx.Message,
                requestId,
                errorCode = "INVALID_ARGUMENT",
                statusCode = StatusCodes.Status400BadRequest
            },
            _ => new
            {
                success = false,
                message = "An unexpected error occurred",
                requestId,
                errorCode = "INTERNAL_SERVER_ERROR",
                statusCode = StatusCodes.Status500InternalServerError
            }
        };

        context.Response.StatusCode = response.statusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}
