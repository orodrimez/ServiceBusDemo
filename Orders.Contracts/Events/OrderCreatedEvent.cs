namespace Orders.Contracts.Events;
public class OrderCreatedEvent
{
    public string OrderId { get; init; }
    public string CustomerName { get; init; } = default!;
    public decimal Amount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

