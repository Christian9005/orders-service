using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrdersApplication.DTOs;
using OrdersApplication.Services;
using OrdersDomain.Entities;
using OrdersDomain.Exceptions;
using OrdersInfrastructure.Context;
using OrdersInfrastructure.Persistence;
using Polly;

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
    public async Task<ApiResponse<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Check idempotency
            var existingOrder = await unitOfWork.Orders
                .GetByIdempotencyKeyAsync(request.IdempotencyKey);

            if (existingOrder != null)
            {
                logger.LogInformation(
                    "Order with IdempotencyKey {IdempotencyKey} already exists. Returning existing order.",
                    request.IdempotencyKey);

                return ApiResponse<OrderResponse>.Ok(
                    OrderResponse.FromEntity(existingOrder),
                    "Order already exists",
                    request.RequestId);
            }

            // 2. Log the start of the process
            await LogAudit(null, request.RequestId, request.Usuario,
                "Order_CreationStarted", "Starting order creation process");

            // 3. Validate client with external service
            logger.LogInformation("Validating client with external service: {ClienteId}",
                request.ClienteId);

            var isValidClient = await validationService.ValidateClientAsync(
                    request.ClienteId, cancellationToken);

            if (!isValidClient)
            {
                await LogAudit(null, request.RequestId, request.Usuario,
                    "Order_ValidationFailed", "Client validation failed");

                return ApiResponse<OrderResponse>.Error(
                    $"Client {request.ClienteId} is not valid",
                    request.RequestId,
                    "INVALID_CLIENT");
            }

            ApiResponse<OrderResponse> result = null;
            var strategy = unitOfWork.Context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // 4. Initialize order creation
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    // 5. Create order and details
                    var order = Order.Create(
                        request.ClienteId,
                        request.Usuario,
                        request.RequestId,
                        request.IdempotencyKey);

                    foreach (var item in request.Items)
                    {
                        order.AddDetail(item.ProductoId, item.Cantidad, item.Precio);
                    }

                    // 6. Validate order
                    order.Validate();

                    order.MarkAsProcessed();

                    // 7. Save order
                    await unitOfWork.Orders.AddAsync(order);
                    await unitOfWork.CommitAsync();

                    await LogAudit(order.Id, request.RequestId, request.Usuario,
                        "Order_Created", $"Order created successfully. Total: {order.Total}");

                    logger.LogInformation(
                        "Order created successfully. OrderId: {OrderId}, RequestId: {RequestId}",
                        order.Id, order.RequestId);

                    order.MarkAsCompleted();
                    await unitOfWork.Orders.UpdateAsync(order);
                    await unitOfWork.SaveChangesAsync();

                    result = ApiResponse<OrderResponse>.Ok(
                        OrderResponse.FromEntity(order),
                        "Order created successfully",
                        request.RequestId);
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackAsync();
                    throw;
                }
            });

            return result;
        }
        catch (DomainException domainEx)
        {
            await LogAudit(null, request.RequestId, request.Usuario,
                "Order_DomainError", $"Domain error: {domainEx.Message}");

            logger.LogError(domainEx, "Domain error {Error}", domainEx.Message);

            return ApiResponse<OrderResponse>.Error(
                domainEx.Message,
                request.RequestId,
                domainEx.ErrorCode);
        }
        catch (Exception ex)
        {
            await LogAudit(null, request.RequestId, request.Usuario,
                "Order_CreationFailed", $"Error: {ex.Message}");

            logger.LogError(ex, "Error creating order. RequestId: {RequestId}",
                request.RequestId);

            return ApiResponse<OrderResponse>.Error(
                "An error occurred while creating the order",
                request.RequestId,
                "INTERNAL_ERROR");
        }
    }

    private async Task LogAudit(int? orderId, Guid requestId, string usuario, string occurence, string description, string level = "Info")
    {
        var log = AuditLog.Create(orderId, requestId, usuario, occurence, description, level);
        await unitOfWork.AuditLogs.AddAsync(log);
        await unitOfWork.SaveChangesAsync();
    }
}
