//How would you design a backend system that consumes incoming Kafka messages and routes them to different storage engines—like PostgreSQL or Cassandra—based on attributes attached inside the Kafka Message Headers, without hardcoding a massive, fragile switch statement inside your consumer loop?
// Implement OCP

// The application cannot anticipate the exact concrete type of class it needs to 
//construct until runtime, how can we achieve this?
//Factory method pattern

using System;
using System.Collections.Generic;
using System.Linq;

public interface IKafkaProcessor
{
    void Process(string payload);
}

public class PostgresProcessor : IKafkaProcessor
{
    public void Process(string payload)
    {
        Console.WriteLine($"Writing payload to PostgreSQL database: {payload}");
    }
}

public class CassandraProcessor : IKafkaProcessor
{
    public void Process(string payload)
    {
        Console.WriteLine($"Writing payload to Cassandra database: {payload}");
    }
}

public abstract class MessageHandler
{
    // INTERVIEW TIP: Exposing an identifier strategy pattern property allows the DI consumer to dynamically group types without using reflection or rigid switch blocks.
    public abstract string TargetDatabaseType { get; }

    // INTERVIEW TIP: This is the core Factory Method. Deferring instantiation to subclasses guarantees strict adherence to the Open-Closed Principle (OCP).
    public abstract IKafkaProcessor CreateProcessor();

    // INTERVIEW TIP: This is the template execution method. The base class controls the runtime lifecycle workflow while knowing nothing about the concrete objects created.
    public void HandleMessage(string messagePayload)
    {
        IKafkaProcessor processor = CreateProcessor();
        processor.Process(messagePayload);
    }
}

public class PostgresMessageHandler : MessageHandler
{
    public override string TargetDatabaseType => "postgres";
    public override IKafkaProcessor CreateProcessor()
    {
        return new PostgresProcessor();
    }
}

public class CassandraMessageHandler : MessageHandler
{
    public override string TargetDatabaseType => "cassandra";
    public override IKafkaProcessor CreateProcessor()
    {
        throw new CassandraProcessor();
    }
}

public interface IKafkaConsumerEngine
{
    void Consume(string targetDBHeader, string payload);
}

public class KafkaConsumerEngine : IKafkaConsumerEngine
{
    private readonly Dictionary<string, MessageHandler> _handlers;

    // INTERVIEW TIP: Injecting IEnumerable<T> demonstrates an intermediate grasp of .NET IoC container features for registering multiple concrete implementations.
    public KafkaConsumerEngine(IEnumerable<MessageHandler> handlers)
    {
       _handlers = handlers.ToDictionary(
        h => h.TargetDatabaseType,
        h => h,
        StringComparer.OrdinalIgnoreCase
       ); 
    }

    public void Consume(string targetDbHeader, string payload)
    {
        if (_handlers.TryGetValue(targetDbHeader, out var handler))
        {
            handler.HandleMessage(payload);
        }
        else
        {
            throw new ArgumentException($"No handler found for target-db header value: {targetDbHeader}");
        }
    }
}

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();

        // INTERVIEW TIP: Registering multiple classes to the same abstract type allows .NET to group them into an IEnumerable sequence automatically.
        services.AddTransient<MessageHandler, PostgresMessageHandler>();
        services.AddTransient<MessageHandler, CassandraMessageHandler>();

        services.AddSingleton<IKafkaConsumerEngine, KafkaConsumerEngine>();

        var serviceProvider = services.BuildServiceProvider();

        var engine = serviceProvider.GetRequiredService<IKafkaConsumerEngine>();

        engine.Consume("postgres", "{\"userId\": 101, \"action\": \"login\"}");
        engine.Consume("cassandra", "{\"sensorId\": 4004, \"reading\": 23.8}");
    }
}


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;

// namespace EnterpriseStreamProcessor
// {
//     // ==========================================
//     // 1. DOMAIN ABSTRACTIONS & PRODUCTS
//     // ==========================================

//     public interface IKafkaProcessor
//     {
//         Task ProcessAsync(string payload, CancellationToken cancellationToken);
//     }

//     public class PostgresProcessor : IKafkaProcessor
//     {
//         private readonly string _connectionString;
//         private readonly ILogger<PostgresProcessor> _logger;

//         // INTERVIEW TIP: Real production processors receive their own dependencies (connections, logging) via DI, not manual instantiation.
//         public PostgresProcessor(string connectionString, ILogger<PostgresProcessor> logger)
//         {
//             _connectionString = connectionString;
//             _logger = logger;
//         }

//         public async Task ProcessAsync(string payload, CancellationToken cancellationToken)
//         {
//             _logger.LogInformation("Writing payload to PostgreSQL instance using target connection string.");
//             await Task.Delay(10, cancellationToken); // Simulating explicit asynchronous I/O operations
//         }
//     }

//     public class CassandraProcessor : IKafkaProcessor
//     {
//         private readonly string _clusterContactPoint;
//         private readonly ILogger<CassandraProcessor> _logger;

//         public CassandraProcessor(string clusterContactPoint, ILogger<CassandraProcessor> logger)
//         {
//             _clusterContactPoint = clusterContactPoint;
//             _logger = logger;
//         }

//         public async Task ProcessAsync(string payload, CancellationToken cancellationToken)
//         {
//             _logger.LogInformation("Writing payload to Cassandra cluster contact point.");
//             await Task.Delay(12, cancellationToken); // Simulating async distributed cluster I/O
//         }
//     }

//     // ==========================================
//     // 2. THE FACTORY METHOD PATTERN CORE
//     // ==========================================

//     public abstract class MessageHandler
//     {
//         // INTERVIEW TIP: String identifier strategy property mapped dynamically into our routing dictionary lookup inside the engine.
//         public abstract string TargetDatabaseType { get; }
        
//         // INTERVIEW TIP: The official Factory Method definition. Object instantiation is deferred entirely to specific subclass overrides.
//         public abstract IKafkaProcessor CreateProcessor();

//         // INTERVIEW TIP: Template Workflow Method. Encapsulates runtime execution rules uniformly while knowing nothing of concrete instances.
//         public async Task HandleMessageAsync(string messagePayload, CancellationToken cancellationToken)
//         {
//             IKafkaProcessor processor = CreateProcessor();
//             await processor.ProcessAsync(messagePayload, cancellationToken);
//         }
//     }

//     public class PostgresMessageHandler : MessageHandler
//     {
//         private readonly IServiceProvider _serviceProvider;
//         public override string TargetDatabaseType => "postgres";

//         // INTERVIEW TIP: Using IServiceProvider inside factory subclasses allows fetching complex dependencies safely without polluting the base class contract.
//         public PostgresMessageHandler(IServiceProvider serviceProvider)
//         {
//             _serviceProvider = serviceProvider;
//         }

//         public override IKafkaProcessor CreateProcessor()
//         {
//             var config = _serviceProvider.GetRequiredService<IConfiguration>();
//             var logger = _serviceProvider.GetRequiredService<ILogger<PostgresProcessor>>();
//             var connString = config.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Missing Postgres Connection String");
            
//             return new PostgresProcessor(connString, logger);
//         }
//     }

//     public class CassandraMessageHandler : MessageHandler
//     {
//         private readonly IServiceProvider _serviceProvider;
//         public override string TargetDatabaseType => "cassandra";

//         public CassandraMessageHandler(IServiceProvider serviceProvider)
//         {
//             _serviceProvider = serviceProvider;
//         }

//         public override IKafkaProcessor CreateProcessor()
//         {
//             var config = _serviceProvider.GetRequiredService<IConfiguration>();
//             var logger = _serviceProvider.GetRequiredService<ILogger<CassandraProcessor>>();
//             var contactPoint = config["CassandraSettings:ContactPoint"] ?? throw new InvalidOperationException("Missing Cassandra Contact Point");
            
//             return new CassandraProcessor(contactPoint, logger);
//         }
//     }

//     // ==========================================
//     // 3. ENGINE ROUTER & BACKGROUND WORKER
//     // ==========================================

//     public interface IKafkaConsumerEngine
//     {
//         Task ConsumeAsync(string targetDbHeader, string payload, CancellationToken cancellationToken);
//     }

//     public class KafkaConsumerEngine : IKafkaConsumerEngine
//     {
//         private readonly Dictionary<string, MessageHandler> _handlers;
//         private readonly ILogger<KafkaConsumerEngine> _logger;

//         // INTERVIEW TIP: Multi-instance injection via IEnumerable<T> maps every handler registered inside the service container cleanly.
//         public KafkaConsumerEngine(IEnumerable<MessageHandler> handlers, ILogger<KafkaConsumerEngine> logger)
//         {
//             _logger = logger;
//             // INTERVIEW TIP: Materializing into a structured Dictionary inside the constructor guarantees efficient O(1) loop evaluations later.
//             _handlers = handlers.ToDictionary(
//                 h => h.TargetDatabaseType,
//                 h => h,
//                 StringComparer.OrdinalIgnoreCase
//             );
//         }

//         public async Task ConsumeAsync(string targetDbHeader, string payload, CancellationToken cancellationToken)
//         {
//             if (_handlers.TryGetValue(targetDbHeader, out var handler))
//             {
//                 await handler.HandleMessageAsync(payload, cancellationToken);
//             }
//             else
//             {
//                 _logger.LogError("Message dropped. Unrecognized infrastructure target header routing token: {Header}", targetDbHeader);
//                 throw new ArgumentException($"No valid concrete handler resolved for header: {targetDbHeader}");
//             }
//         }
//     }

//     // INTERVIEW TIP: In production apps, Kafka handlers run inside a non-blocking, continuous Background Hosted Service lifecycle loop.
//     public class KafkaConsumerWorker : BackgroundService
//     {
//         private readonly IKafkaConsumerEngine _consumerEngine;
//         private readonly ILogger<KafkaConsumerWorker> _logger;

//         public KafkaConsumerWorker(IKafkaConsumerEngine consumerEngine, ILogger<KafkaConsumerWorker> logger)
//         {
//             _consumerEngine = consumerEngine;
//             _logger = logger;
//         }

//         protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//         {
//             _logger.LogInformation("Kafka background stream consumer worker started successfully.");

//             while (!stoppingToken.IsCancellationRequested)
//             {
//                 try
//                 {
//                     // Simulated streaming data input loops running constantly over connection context pipelines
//                     await _consumerEngine.ConsumeAsync("postgres", "{\"deviceId\": 883, \"metric\": \"temp\"}", stoppingToken);
//                     await Task.Delay(1000, stoppingToken); 
                    
//                     await _consumerEngine.ConsumeAsync("cassandra", "{\"logId\": 9921, \"status\": \"nominal\"}", stoppingToken);
//                     await Task.Delay(1000, stoppingToken);
//                 }
//                 catch (Exception ex) when (ex is not OperationCanceledException)
//                 {
//                     _logger.LogError(ex, "An error occurred while evaluating incoming streaming telemetry data packets.");
//                 }
//             }
//         }
//     }

//     // ==========================================
//     // 4. MAIN ENTRY POINT (PROGRAM.CS)
//     // ==========================================

//     public class Program
//     {
//         public static async Task Main(string[] args)
//         {
//             // INTERVIEW TIP: The built-in .NET Generic Host initializes global Logging, Configuration loading, and Dependency Lifecycles automatically.
//             var host = Host.CreateDefaultBuilder(args)
//                 .ConfigureServices((hostContext, services) =>
//                 {
//                     // Registering individual concrete pattern abstractions to the standard container sequence interface
//                     services.AddTransient<MessageHandler, PostgresMessageHandler>();
//                     services.AddTransient<MessageHandler, CassandraMessageHandler>();

//                     // Registering core engine infrastructure orchestrator patterns
//                     services.AddSingleton<IKafkaConsumerEngine, KafkaConsumerEngine>();

//                     // Injecting long running asynchronous system background hosted worker engines
//                     services.AddHostedService<KafkaConsumerWorker>();
//                 })
//                 .Build();

//             // RunAsync blocks the main thread sequence executing loop pipelines gracefully until an OS termination interrupt is thrown.
//             await host.RunAsync();
//         }
//     }
// }
