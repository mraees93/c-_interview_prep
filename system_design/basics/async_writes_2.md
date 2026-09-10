# ✉️ Production Guide: Scalable & High-Availability RabbitMQ Architecture

## 🧬 1. The Core Infrastructure Design (The Mailroom Contract)

1. **The Infrastructure Boundary:** RabbitMQ operates as an independent, stateful message broker service. It maintains raw byte buffers on disk or RAM (Queues) and routes messages based on rule criteria (Exchanges). It does not execute application code.
2. **The Worker Boundary:** Consumers are external application runtimes (like a C# .NET `BackgroundService` wrapper) running on distinct process boundaries. They pull payloads over an active TCP network connection channel.
3. **The Data Safety Contract (`ACK`):** RabbitMQ retains a message in safe storage until the external worker emits an explicit programmatic acknowledgment string (`BasicAck`). If a worker process crashes mid-execution, the network socket drops, and RabbitMQ re-queues the payload instantly for another worker.

---

## 💻 2. Implementing the C# .NET Worker (`BackgroundService`)

1. **The Long-Lived Daemon Boundary:** Create an independent .NET worker class inheriting from `BackgroundService` to act as a continuous event consumer loop.
2. **The Dependency Isolation Rule:** Never inject a short-lived database container context (`DbContext`) directly into the worker's constructor. This triggers a fatal **Captive Dependency** leak. Inject an `IServiceScopeFactory` to generate micro-sandbox execution scopes at runtime.
3. **The Capacity Controller Configuration:** Always declare `channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false)` inside your queue connection sequence. This blocks RabbitMQ from flooding a single socket buffer with messages, enforcing a resilient **Pull-on-Demand** workflow.

```csharp
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LexisNexisWorkspace.Infrastructure.Messaging;

public class LegalCaseConsumerWorker : BackgroundService
{
    private readonly ILogger<LegalCaseConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory; 
    private IConnection? _connection;
    private IModel? _channel;
    private const string QueueName = "legal.cases.ingestion";

    public LegalCaseConsumerWorker(ILogger<LegalCaseConsumerWorker> logger, IServiceScopeFactory _factory)
    {
        _logger = logger;
        _scopeFactory = _factory;
        InitializeBrokerChannel();
    }

    private void InitializeBrokerChannel()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        
        // 🚨 STEP 3: Enforce the explicit Prefetch Limit rule to enable load balancing
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            bool processingSuccess = false;

            // 🧼 Open an isolated heap scope per message invocation frame
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LegalDbContext>();
                try
                {
                    var payload = JsonSerializer.Deserialize<CaseDocumentDto>(message);
                    if (payload != null)
                    {
                        dbContext.CaseDocuments.Add(new CaseDocumentEntity { Title = payload.Title });
                        await dbContext.SaveChangesAsync(stoppingToken);
                        processingSuccess = true; 
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Domain transaction fatal drop.");
                }
            } 

            if (processingSuccess)
            {
                // Emit confirmation down the wire to clear the bytes from broker memory
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
```

### ☁️ Cloud Ingestion Scaling Mechanics

* **The Rule:** Focus entirely on building a single, highly performant C# `BackgroundService` project mapped to your ingestion queue parameters. 
* **The Infrastructure Execution:** Package this unified worker footprint inside a single Docker image file. Set a hard production baseline of exactly 2 container task instances to satisfy high availability boundaries.
* **The Automated Scale Pipeline:** Leverage event-driven scaling mechanics (like KEDA or AWS CloudWatch triggers) to automatically monitor queue metrics. The cloud framework dynamically boots extra competing consumer container instances to smash traffic spikes and safely deconstructs them when traffic cools down.

---

## 👥 3. Multi-Worker Competing Consumer Engine

1. **The Decentralised Scale Rule:** Do not write local C# thread-spawning loops or task-parallel managers inside a single application process to scale out workload threads. 
2. **The Competing Consumer Pattern:** Spin up multiple separate, identical instances of your worker application container pointing to the exact same queue name target string. 
3. **The Dynamic Round-Robin Balancer:** RabbitMQ automatically treats these split connections as **Competing Consumers**. Paired with your `prefetchCount: 1` setting, RabbitMQ distributes incoming messages one-by-one to whichever worker thread pool bench is currently free, enabling zero-downtime application updates and perfect processing efficiency.

---

## 🐳 4. The Docker Containerization Layer

1. **Isolation Architecture:** package your single-threaded `BackgroundService` application inside an optimized, multi-stage **Dockerfile** built over a lightweight `.NET Runtime Alpine` linux base image layer.
2. **The Central Compose Configuration:** Create a `docker-compose.yml` file orchestration matrix that maps out your isolated processing images relative to a baseline RabbitMQ server hub container.
3. **The Scale Command Execution:** To achieve local multi-worker high availability, spin up your compose file network infrastructure, and then execute the scale command directly against the worker engine instance layout:
   ```bash
   # Dynamically boots 3 completely independent, competing consumer containers instantly
   docker compose up -to --scale case-worker-service=3
   ```

---

## ☁️ 5. Enterprise Cloud Deployments (Azure / AWS)

1. **The AWS Deployment Pipeline (ECS / EKS):** Build and push your worker Docker image down to Amazon Elastic Container Registry (ECR). Deploy the workers as an **ECS Service** running on AWS Fargate serverless infrastructure nodes.
2. **The Azure Deployment Pipeline (ACA / AKS):** Push your worker Docker image to Azure Container Registry (ACR). Deploy the containers inside **Azure Container Apps (ACA)** or an Azure Kubernetes Service cluster node matrix.
3. **The Decoupled Broker Isolation:** In enterprise-grade cloud environments, do not run standard RabbitMQ inside a single raw VM. Spin up a highly available, managed cloud engine instance—such as **Amazon MQ for RabbitMQ** or **Azure Service Bus (AMQP Standard)**—to serve as your hardened infrastructure routing backbone.

---

## ⚖️ 6. The Scale-Out Matrix: Vertical vs. Horizontal Boundaries

1. **Vertical Scaling (Scaling Up):** Modifying the underlying hardware limits of a single host container (e.g., changing an ECS task profile from 0.5 vCPU to 4 vCPU or expanding VM RAM limits).
2. **Vertical Use-Case/Limit:** Best used when your domain workers are hitting thread locks processing highly complex, single-threaded calculations or memory-heavy calculations. It does **not** provide high availability; if that host machine experiences a power failure, your system goes offline.
3. **Horizontal Scaling (Scaling Out):** Adding more distinct physical worker container nodes across separate cloud infrastructure endpoints.
4. **Horizontal Use-Case/Limit:** The standard choice for data ingestion pipelines. Leveraging **KEDA (Kubernetes Event-driven Autoscaling)** allows Azure Container Apps or AWS EKS clusters to automatically monitor your RabbitMQ queue depth. If the queue length spikes to 10,000 backlogged messages, the cloud architecture instantly clones your worker container footprint from 2 instances up to 20 instances across different machines, scaling down back to a lightweight baseline when the queue hits zero.
