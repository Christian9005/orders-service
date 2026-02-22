using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersApplication.Commands;
using OrdersApplication.DTOs;

namespace OrdersApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly ILogger<OrdersController> logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var requestId = HttpContext.Items["RequestId"] as Guid? ?? Guid.NewGuid();
        var idempotencyKey = HttpContext.Items["IdempotencyKey"] as string
            ?? Guid.NewGuid().ToString();

        logger.LogInformation(
            "Creating order for client {ClienteId}. RequestId: {RequestId}",
            request.ClienteId, requestId);

        var command = new CreateOrderCommand
        {
            ClienteId = request.ClienteId,
            Usuario = request.Usuario,
            Items = request.Items,
            RequestId = requestId,
            IdempotencyKey = idempotencyKey
        };

        var result = await mediator.Send(command, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(CreateOrder), result);
    }
}
