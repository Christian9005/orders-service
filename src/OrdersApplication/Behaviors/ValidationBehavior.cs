using FluentValidation;
using MediatR;
using OrdersApplication.DTOs;

namespace OrdersApplication.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errors = failures.Select(f => new ErrorDetail
            {
                Code = "VALIDATION_ERROR",
                Message = f.ErrorMessage,
                Field = f.PropertyName
            }).ToList();

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                var responseType = typeof(TResponse).GetGenericArguments()[0];
                var method = typeof(ApiResponse<>).MakeGenericType(responseType)
                    .GetMethod(nameof(ApiResponse<object>.ValidationError));

                var response = method?.Invoke(null, new object[] { errors, Guid.NewGuid() });
                return (TResponse)response!;
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}
