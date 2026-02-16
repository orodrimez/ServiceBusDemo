using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Orders.Contracts.Events;

namespace Orders.Api.Services;

public class OrderPublisher
{
    private readonly ServiceBusClient _client;
    private readonly string _queueName;

    public OrderPublisher(ServiceBusClient client, IConfiguration config)
    {
        _client = client;
        _queueName = config["ServiceBus:QueueName"]!;
    }

    public async Task PublishAsync(OrderCreatedEvent orderEvent)
    {
        var sender = _client.CreateSender(_queueName);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(orderEvent))
        {
            ContentType = "application/json",
            MessageId = orderEvent.OrderId.ToString(),
            CorrelationId = orderEvent.OrderId.ToString()
        };

        await sender.SendMessageAsync(message);
    }
}
