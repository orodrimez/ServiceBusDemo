using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orders.Contracts.Events;

namespace Orders.Worker;

public class Worker : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<Worker> _logger;

    public Worker(ServiceBusClient client, IConfiguration config, ILogger<Worker> logger)
    {
        _logger = logger;

        _processor = client.CreateProcessor(
            config["ServiceBus:QueueName"],
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 5,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
            });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += ProcessMessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();

            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

            _logger.LogInformation(
                "Processing Order {OrderId} for {Customer}",
                orderEvent?.OrderId,
                orderEvent?.CustomerName);

            // Simulacion
            await Task.Delay(1000);

            await args.CompleteMessageAsync(args.Message);

            _logger.LogInformation(
                "Order {OrderId} completed",
                orderEvent?.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");

            // Reintento
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception,"Service Bus error. Entity: {EntityPath}",args.EntityPath);
        return Task.CompletedTask;
    }
}
