namespace OrdersApi.Middlewares;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate next;
    private const string IdempotencyKeyHeaderName = "X-Idempotency-Key";

    public IdempotencyMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != HttpMethods.Post &&
            context.Request.Method != HttpMethods.Patch &&
            context.Request.Method != HttpMethods.Put)
        {
            await next(context);
            return;
        }

        var idempotencyKey = context.Request.Headers[IdempotencyKeyHeaderName].ToString();

        if (string.IsNullOrEmpty(idempotencyKey))
        {
            idempotencyKey = Guid.NewGuid().ToString();
        }

        context.Items["IdempotencyKey"] = idempotencyKey;

        await next(context);
    }
}
