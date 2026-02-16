using Azure.Messaging.ServiceBus;
using Orders.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new ServiceBusClient(
        config["ServiceBus:ConnectionString"]);
});
builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
