# Orders System — Azure Service Bus Event-Driven Architecture

## 📌 Overview

This project demonstrates a cloud-native, event-driven architecture built with:

- ASP.NET Core 8
- Azure Service Bus (Queue)
- .NET Background Worker Service
- Application Insights
- Clean contract separation

The system processes orders asynchronously using a message queue to achieve:

- Decoupling
- Scalability
- Resiliency
- Observability

---

# 🏗 Architecture

## Flow

Client  
↓  
Orders.Api  
↓  
Azure Service Bus (orders-queue)  
↓  
Orders.Worker  

Orders.Api ─────► Application Insights  
Orders.Worker ───► Application Insights  

---

## Key Concepts Implemented

- Event-driven architecture  
- Asynchronous message processing  
- Manual message completion  
- Automatic retry handling  
- Dead Letter Queue (DLQ)  
- Distributed logging & telemetry  

---

# 📦 Solution Structure

```
OrdersSystem
│
├── Orders.Api
├── Orders.Worker
└── Orders.Contracts
```

## Projects

### Orders.Api
- ASP.NET Core 8 Web API
- Publishes `OrderCreatedEvent`
- Returns HTTP 202 Accepted
- Sends telemetry to Application Insights

### Orders.Worker
- .NET BackgroundService
- Consumes messages from Service Bus
- Manual `CompleteMessageAsync`
- Retry via `AbandonMessageAsync`
- Sends telemetry to Application Insights

### Orders.Contracts
- Shared event contracts
- Contains `OrderCreatedEvent`
- Prevents tight coupling between services

---

# ☁ Azure Resources Used

- Service Bus Namespace (Standard Tier)
- Queue: `orders-queue`
- Dead Letter Queue (automatic)
- Application Insights

## Queue Configuration

- Max Delivery Count: 5  
- Partitioning: Enabled  
- Lock Duration: 30 seconds  

If a message fails 5 times, it is automatically moved to the Dead Letter Queue.

---

# 🚀 How It Works

## 1️⃣ Orders.Api

- Receives HTTP POST `/orders`
- Creates `OrderCreatedEvent`
- Publishes event to Service Bus
- Returns `202 Accepted`

Message includes:

- MessageId  
- CorrelationId  
- ContentType = application/json  

The API does not process business logic directly.

---

## 2️⃣ Azure Service Bus

Acts as a message broker:

- Stores messages
- Manages retries
- Handles dead-lettering
- Enables decoupling between API and Worker

---

## 3️⃣ Orders.Worker

- Listens to `orders-queue`
- Deserializes message
- Simulates processing
- Manually completes message

If processing fails:

- Message is abandoned
- Service Bus retries automatically
- After 5 attempts → Dead Letter Queue

---

# 🔍 Observability

Both API and Worker are integrated with Application Insights, providing:

- Request telemetry
- Dependency tracking
- Trace logs
- Exception tracking
- Distributed correlation

Example KQL query:

```kusto
traces
| order by timestamp desc
```

---

# 🛠 Running Locally

## 1️⃣ Configure appsettings.json

Add your Service Bus and Application Insights connection strings:

```
"ServiceBus": {
  "ConnectionString": "YOUR_SERVICE_BUS_CONNECTION_STRING",
  "QueueName": "orders-queue"
},
"ApplicationInsights": {
  "ConnectionString": "YOUR_APP_INSIGHTS_CONNECTION_STRING"
}
```

## 2️⃣ Run API

```
dotnet run --project Orders.Api
```

## 3️⃣ Run Worker

```
dotnet run --project Orders.Worker
```

## 4️⃣ Test

Send POST request:

```
POST /orders
{
  "customerName": "Oscar",
  "amount": 150
}
```

---

# 🧠 Architectural Decisions

- Separation of concerns between API and Worker  
- Contract-first design  
- Message-based communication  
- Manual control of message lifecycle  
- Centralized logging and monitoring  
- Designed for horizontal scalability  

---

# 🔮 Possible Improvements

- Managed Identity + Azure Key Vault  
- Dockerization  
- CI/CD Pipeline  
- Topic + Subscriptions  
- Outbox Pattern  
- Persistent storage integration  
- Autoscaling with Azure Container Apps  

---

# 📌 Summary

This project demonstrates a production-style asynchronous architecture using Azure Service Bus and .NET.

It highlights:

- Clean architecture principles  
- Event-driven design  
- Resilient message processing  
- Observability best practices  

---

## Author

Built as a hands-on cloud architecture implementation using .NET 8 and Azure.
