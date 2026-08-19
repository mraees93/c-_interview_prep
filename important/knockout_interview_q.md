# Technical Panel Knockout Filters - Defenses & Mechanics

This module isolates the highest-risk screening scenarios used by panels to immediately evaluate intermediate engineering maturity. Falling into these traps results in an immediate fail.

---

## 1. The Global Async Starvation Knockout

### The Panel Trap
"We have a legacy endpoint handling legal document conversions. To save time, a developer wrapped an asynchronous compression task inside a synchronous method using `.Result` or `.Wait()`. It works perfectly in local testing with a single user, but when we deploy it to production under heavy traffic, the entire API freezes and stops responding completely. Why?"

```csharp
public string GetCompressedDocumentJson(string docId)
{
    var task = _compressionService.CompressAsync(docId);
    return task.Result; 
}
```

### The Defensive Response
*   **The Disaster:** Causes **Thread Pool Starvation** and an immediate deadlock. The request thread blocks waiting synchronously on `.Result`, while the async task tries to use the same thread via the `SynchronizationContext` to finish up. Under traffic, the system runs out of execution threads entirely, freezing the API globally.
*   **The Fix:** Maintain a non-blocking asynchronous pipeline from top to bottom. Swap the blocking parameter with an explicit `await` modifier and change the method signature to return a `Task`.

```csharp
public async Task<string> GetCompressedDocumentJsonAsync(string docId)
{
    return await _compressionService.CompressAsync(docId);
}
```

---

## 2. The Multi-Threaded State Corruption Knockout

### The Panel Trap
"We have a high-frequency background service that logs document lookup metadata. To optimize operations, we use a single instance of a standard `Dictionary<string, string>` as a shared cache inside a Singleton class. Multiple concurrent threads read and write token items to this dictionary simultaneously. What occurs under heavy traffic?"

```csharp
public class TokenCache
{
    private readonly Dictionary<string, string> _tokens = new();

    public void AddToken(string id, string token)
    {
        _tokens[id] = token; 
    }
}
```

### The Defensive Response
*   **The Disaster:** A standard `Dictionary<K,V>` is **not thread-safe**. Concurrent thread updates corrupt the internal structural hash buckets and array resizing operations, leading to missing data records, internal memory drift, or a continuous CPU spike to 100%.
*   **The Fix:** Implement a native **`ConcurrentDictionary<K,V>`**. This introduces fine-grained, bucket-level lock striping under the hood to ensure multiple executing threads can safely write to independent data addresses simultaneously.

```csharp
private readonly ConcurrentDictionary<string, string> _tokens = new();
```

---

## 3. The Structural Data Mutation Knockout

### The Panel Trap
"We are processing a collection of legal metadata updates inside a tight parsing execution loop. A developer created a mutable custom `struct` to track the updates, but noticed that some data fields are failing to update correctly in the final output collection. Why is this happening?"

```csharp
public struct CaseMetadata
{
    public string Status { get; set; } 
}
```

### The Defensive Response
*   **The Disaster:** Structs are **Value Types** managed on the Stack. Passing a mutable struct into methods or parsing loops forces the runtime to make a **complete copy of the entire memory footprint across the stack**, meaning mutations target a hidden copy while leaving the original instance completely unchanged.
*   **The Fix:** Enforce immutability. Apply the **`readonly struct`** constraint to force fields to use `init`-only setters, or migrate the domain model to a reference-tracked `record class`.

```csharp
public readonly struct CaseMetadata
{
    public string Status { get; init; }
}
```

---

## 4. The Database Connection Pool Starvation Knockout

### The Panel Trap
"We have a high-traffic .NET API endpoint that retrieves user settings from SQL Server. Under load testing, after processing around 100 concurrent requests, the entire API stops responding and starts throwing a `TimeoutException` stating: *'The timeout period elapsed prior to obtaining a connection from the pool.'* The database itself is idling at 5% CPU. What did the developer do wrong?"

```csharp
public async Task<UserSettings> GetSettingsAsync(int userId)
{
    var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync();
    
    var command = new SqlCommand("SELECT * FROM Settings WHERE UserId = @id", connection);
    command.Parameters.AddWithValue("@id", userId);
    
    var reader = await command.ExecuteReaderAsync(); 
    return MapSettings(reader);
}
```

### The Defensive Response
*   **The Disaster:** The developer leaked unmanaged network resources, triggering **Connection Pool Starvation**. ADO.NET pools are capped at a default limit (usually 100 connections). Because unclosed `SqlConnection` sockets are not returned to the pool, incoming request threads block waiting indefinitely until they time out and crash the API.
*   **The Fix:** Wrap the connections, commands, and reader allocation scripts inside block-scoped **`using` statements**. This forces an automatic `try/finally` context that guarantees unmanaged sockets close and return to the pool immediately upon exit.

```csharp
public async Task<UserSettings> GetSettingsAsync(int userId)
{
    using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync();
    
    using var command = new SqlCommand("SELECT * FROM Settings WHERE UserId = @id", connection);
    command.Parameters.AddWithValue("@id", userId);
    
    using var reader = await command.ExecuteReaderAsync();
    return MapSettings(reader);
}
```

## 5. The Entity Framework Memory Leak Knockout

### The Panel Trap
"We have an administrative background tracking service that reads millions of historical legal logs using Entity Framework Core, applies a rule validation, and stores the results elsewhere. Over a 4-hour runtime window, the application's RAM usage climbs continuously until the container hits its memory limit and gets forcefully killed by the OS (OOM Out of Memory crash). Why is EF Core hoarding memory?"

```csharp
public async Task ProcessAuditLogsAsync()
{
    var logs = await _context.AuditLogs.Where(l => l.IsProcessed == false).ToListAsync();
    
    foreach (var log in logs)
    {
        ValidateLogPayload(log); 
    }
}
```

### The Defensive Response
*   **The Disaster:** Causes an internal tracking memory leak. By default, EF Core creates and holds data tracking snapshots of every entity fetched inside the `DbContext` instance [Glassdoor]. Processing millions of logs expands this internal tracking cache indefinitely, ballooning the Managed Heap size until the OS kills the process via an Out-Of-Memory (OOM) crash.
*   **The Fix:** Apply the **`.AsNoTracking()`** extension modifier to the LINQ query. This explicitly instructs the ORM to completely bypass state-tracking snapshot generation, keeping memory usage flat and significantly speeding up query execution speeds.

```csharp
var logs = await _context.AuditLogs
    .AsNoTracking() 
    .Where(l => l.IsProcessed == false)
    .ToListAsync();
```

---

## 6. The Precision Financial Corruption Knockout

### The Panel Trap
"We are writing a high-frequency billing calculator module that aggregates fee transactions and subscription rates for corporate law firms. A developer defined the values using the standard `double` primitive type because it executes lightning-fast at the hardware layer. However, after running millions of operations, our auditing software discovers that our calculations are off by fractions of a cent, resulting in subtle financial data corruption. What happened?"

```csharp
public class BillingEngine
{
    public double AggregateSubscriptionFees(double baseRate, double taxPercentage)
    {
        return baseRate * (1.0 + taxPercentage); 
    }
}
```

### The Defensive Response
*   **The Disaster:** Creates compounding precision errors. Primitives like `double`/`float` are base-2 binary floating-point numbers [Glassdoor]. Base-10 fractional decimals (like `0.1` or `0.7`) cannot be represented exactly in binary and are stored as repeating approximations [Glassdoor]. Over millions of continuous operations, these tiny rounding errors accumulate, leading to compounding data corruption.
*   **The Fix:** Enforce the use of the **`decimal`** type (and suffix literals with `m`). The `decimal` type is a 128-bit base-10 structure computed via software emulation, which completely eliminates base-2 fractional rounding anomalies to guarantee 100% mathematical accuracy.

```csharp
public class BillingEngine
{
    public decimal AggregateSubscriptionFees(decimal baseRate, decimal taxPercentage)
    {
        return baseRate * (1.0m + taxPercentage); 
    }
}
```
