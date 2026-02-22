using FluentValidation;
using OrdersApplication.Commands;
using OrdersApplication.DTOs;

namespace OrdersApplication.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0)
            .WithMessage("ClienteId must be greater than 0");

        RuleFor(x => x.Usuario)
            .NotEmpty()
            .WithMessage("Usuario is required")
            .MaximumLength(100)
            .WithMessage("Usuario cannot exceed 100 characters");

        RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required")
                .Must(items => items.Count > 0)
                .WithMessage("Order must have at least one detail");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderDetailValidator());
    }
}

public class OrderDetailValidator : AbstractValidator<OrderDetailRequest>
{
    public OrderDetailValidator()
    {
        RuleFor(x => x.ProductoId)
            .GreaterThan(0)
            .WithMessage("ProductoId must be greater than 0");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("Cantidad must be greater than 0");

        RuleFor(x => x.Precio)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Precio cannot be negative");
    }
}
