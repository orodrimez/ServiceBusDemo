using Microsoft.AspNetCore.Mvc;
using Orders.Api.Services;
using Orders.Contracts.Events;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderPublisher _publisher;

    public OrdersController(OrderPublisher publisher)
    {
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<IResult> Create(CreateOrderRequest request)
    {
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid().ToString(),
            CustomerName = request.CustomerName,
            Amount = request.Amount,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _publisher.PublishAsync(orderEvent);

        return Results.Accepted(orderEvent.OrderId);
    }
}

public record CreateOrderRequest(string CustomerName, decimal Amount);
