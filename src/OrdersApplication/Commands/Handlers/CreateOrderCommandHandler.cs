using MediatR;
using Microsoft.Extensions.Logging;
using OrdersApplication.DTOs;
using OrdersApplication.Services;
using OrdersInfrastructure.Persistence;

namespace OrdersApplication.Commands.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResponse<OrderResponse>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IExternalValidationService validationService;
    private readonly ILogger<CreateOrderCommandHandler> logger;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IExternalValidationService validationService, ILogger<CreateOrderCommandHandler> logger)
    {
        this.unitOfWork = unitOfWork;
        this.validationService = validationService;
        this.logger = logger;
    }
    public Task<ApiResponse<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
