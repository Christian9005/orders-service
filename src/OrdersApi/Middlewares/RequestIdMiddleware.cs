namespace OrdersApi.Middlewares;

public class RequestIdMiddleware
{
    private readonly RequestDelegate next;
    private const string RequestIdHeaderName = "X-Request-Id";

    public RequestIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers[RequestIdHeaderName].ToString();

        if (string.IsNullOrEmpty(requestId))
        {
            requestId = Guid.NewGuid().ToString();
        }

        context.Items["RequestId"] = Guid.Parse(requestId);
        context.Response.Headers[RequestIdHeaderName] = requestId;

        await next(context);
    }
}
