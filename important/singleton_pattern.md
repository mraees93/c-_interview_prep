# 🎭 Design Patterns Vault: The Singleton Pattern
*LexisNexis Cape Town Preparation - Core High-Concurrency Structural Control Module*

---

## 🎭 The Unified Analogy: The House Address Sign (`static`) vs. The Configurable Passage Geyser (Singleton)

To clearly defend your architectural choices to a principal engineer, use your **House Lot and Passage Setup** to separate hardcoded compilation models from runtime object lifecycles:

*   **The `static` Access Modifier (The House Number painted onto the Bricks):** 
    This is like painting the house number **"No. 45"** permanently onto the structural concrete brick wall of your lounge. It is baked directly into the building structure at compile-time. It requires zero setup, but it is rigid, cannot be easily changed or swapped out, and cannot dynamically react to external signals.
*   **The Singleton Pattern (The Configurable Passage Geyser):** A normal class, but the master switchboard (**`Program.cs`**) initializes it exactly once on boot. It acts like the single **Geyser Heater** in your passage. When family members turn on different taps (**concurrent execution threads**), they all draw hot water from this exact same central unit instead of building a brand-new geyser. Because it is a live runtime object, you can configure its timers or manually toggle it on/off—flexibility a hardcoded static brick painting cannot provide.

---

| Core Feature | Requirement | What Makes It So? (The Structural Setup) | The Runtime Disaster (If Broken) | The Specific Error / Symptom |
| :--- | :--- | :--- | :--- | :--- |
| **Stateless (Read-Only)** | **No** (Can hold state) | Has **no internal field variables that change value** after object instantiation. Every operation is pure logic. | **Cross-Request Contamination** | User A sees User B's cached details or private session state on the UI. |
| **Thread-Safe** | **Yes** (Non-negotiable) | Uses **synchronization locks, immutable data types, or concurrent structures** (`Lazy<T>`, `ConcurrentDictionary`). | **Race Conditions & Collection Corruption** | `NullReferenceException` or `IndexOutOfRangeException` under heavy concurrent traffic. |

---

### 🔌 Composition Root: Registering Multiple Unique Singleton Services

```csharp
// Program.cs - Setting up multiple global house appliances
var builder = WebApplication.CreateBuilder(args);

// Singleton 1: Your Passage Geyser (Manages configuration data)
builder.Services.AddSingleton<ILegalConfigurationCache, LegalConfigurationCache>();

// Singleton 2: Your Central Security Alarm (Manages global logging/telemetry)
builder.Services.AddSingleton<ITelemetryBroker, TelemetryBroker>();

// Singleton 3: Your Prepaid Meter (Manages external third-party connection sockets)
builder.Services.AddSingleton<IApiConnectionMultiplexer, ApiConnectionMultiplexer>();

var app = builder.Build();
```

---

## ⚙️ Production Blueprint: Thread-Safe Singleton Options

When a panel asks you to implement a Singleton, you must account for how the instance is created. The native .NET Dependency Injection (IoC) container cannot instantiate a class using a `private` constructor. Use one of the two enterprise patterns below:

### Option A: Framework-Managed Singleton (Recommended for Modern .NET)
You completely drop the private static instance property and the private constructor. You leave the constructor public, and rely entirely on `Program.cs` to ensure only a single instance is ever born and passed down.

```csharp
namespace LexisNexisWorkspace.Services;

public sealed class LegalConfigurationCache : ILegalConfigurationCache
{
    // Public constructor allows the native .NET IoC container to initialize it once on boot
    public LegalConfigurationCache()
    {
    }

    public string GetConfigValue(string key)
    {
        return "CapeTown_Production_Node";
    }
}
```

### Option B: Classic Pattern Self-Managed Instance Bound to DI
If you want to enforce a hard `private` constructor so that no developer can ever call the `new` keyword manually in code, you must wrap it in a thread-safe `Lazy<T>` wrapper and register the exact `.Instance` reference directly inside your composition root.

```csharp
namespace LexisNexisWorkspace.Services;

public sealed class LegalConfigurationCache : ILegalConfigurationCache
{
    private static readonly Lazy<LegalConfigurationCache> _instance = 
        new Lazy<LegalConfigurationCache>(() => new LegalConfigurationCache());

    // Explicit private constructor blocks external code layers from using the 'new' keyword.
    private LegalConfigurationCache()
    {
    }

    // Public global entry point to securely access the single managed Heap instance.
    public static LegalConfigurationCache Instance => _instance.Value;

    public string GetConfigValue(string key)
    {
        return "CapeTown_Production_Node";
    }
}
```
### Option C: Mutable (Stateful) Thread-Safe Implementation
If your Singleton component cannot be read-only and must store globally shared runtime data changes dynamically, you must enforce internal thread safety by swapping standard primitive collections with native concurrent collections.

```csharp
namespace LexisNexisWorkspace.Services;

public sealed class LegalConfigurationCache : ILegalConfigurationCache
{
    private static readonly Lazy<LegalConfigurationCache> _instance =
        new Lazy<LegalConfigurationCache>(() => new LegalConfigurationCache());

    // CRITICAL FOR MUTABILITY: Standard Dictionary will crash under multi-threaded writes.
    // ConcurrentDictionary implements bucket-level lock striping automatically under the hood.
    private readonly ConcurrentDictionary<string, string> _dynamicSettings = new();

    private LegalConfigurationCache()
    {
    }

    public static LegalConfigurationCache Instance => _instance.Value;

    // Mutates state safely across multiple concurrent incoming web request threads
    public void UpdateSetting(string key, string value)
    {
        _dynamicSettings[key] = value;
    }

    // Securely reads runtime state values without locking out thread execution workers
    public string GetConfigValue(string key)
    {
        return _dynamicSettings.TryGetValue(key, out var val) ? val : "CapeTown_Production_Node";
    }
}
```

---

## 🔌 Composition Root Integration: Registration inside `Program.cs`

Depending on which option you selected above, you will integrate your dependency inside your application builder differently:

```csharp
// File path: Program.cs
using LexisNexisWorkspace.Services;

var builder = WebApplication.CreateBuilder(args);

// IF YOU CHOSE OPTION A (Framework-Managed):
// The container finds the public constructor and manages the single instance lifecycle under the hood.
builder.Services.AddSingleton<ILegalConfigurationCache, LegalConfigurationCache>();


// IF YOU CHOSE OPTION B (Classic Self-Managed Pattern):
// You must explicitly feed the container your pre-created private lazy instance.
builder.Services.AddSingleton<ILegalConfigurationCache>(LegalConfigurationCache.Instance);

var app = builder.Build();
app.Run();
```

---

## 🚨 The Critical Intermediate Interview Traps

If you discuss Singleton implementations with a senior architect panel, you must prove you understand their internal infrastructure risks:

### 💥 Trap 1: The Multi-Threaded State Corruption Knockout (Shared Mutable State)
*   **The Disaster:** If your Singleton class stores mutable data configurations inside a standard collection (like a primitive `Dictionary<string, string>`), multiple concurrent background web request threads will attempt to write to that exact same reference address space at the same microsecond. This triggers immediate internal hash array corruption, throwing runtime collection errors or locking your host CPU registers to 100%.
*   **The Fix:** Keep your Singleton component completely **stateless (read-only)**, or enforce internal thread safety by utilizing a native **`ConcurrentDictionary<K, V>`** to implement automatic, bucket-level lock striping under the hood.

### 💥 Trap 2: The Memory Leak Capture Knockout (Captive Dependencies)
*   **The Disaster:** Injecting a short-lived service (like a Scoped Entity Framework Core `DbContext`) straight into the constructor of a long-lived Singleton service. Because your Singleton never dies for the entire lifecycle of the application process, it holds that `DbContext` instance hostage on the Heap forever, leaking database sockets and triggering an eventually fatal **Connection Pool Starvation** crash.
*   **The Fix:** Never mix lifecycles via constructor injection. If a Singleton service absolutely must query the database runtime, inject an **`IServiceScopeFactory`** instead, allowing the code block to open a transient, micro-scoped boundary that disposes of the connection immediately upon method completion:

```csharp
public class LegalConfigurationCache : ILegalConfigurationCache
{
    private readonly IServiceScopeFactory _scopeFactory;

    public LegalConfigurationCache(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<string> GetLiveMetadataAsync(string key)
    {
        // Creates a localized, micro-scoped runtime boundary that exits and cleans up safely
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LegalDbContext>();
        return await context.Settings.FindAsync(key);
    }
}
```

---

# Mutable Singletons: Architectural Guidelines

## 🚀 When to Use a Mutable Singleton
Use a mutable (stateful) singleton when the application requires a **single, globally shared source of truth** that updates dynamically at runtime.

* **In-Memory Caches:** Storing slow-moving configuration data or lookup tables that refresh periodically.
* **Centralized Gateways:** Managing active real-time connections, background task states, or circuit breakers.
* **Global Rate Limiters:** Tracking runtime request volumes or traffic throttling across different incoming threads.

---

## 🛑 When to Avoid
* **Horizontal Scaling:** State stays isolated to a single server instance. If your app runs on multiple nodes, use a distributed store like **Redis**.
* **Request-Specific Data:** Storing user sessions, shopping carts, or transaction details causes cross-request data corruption. Use **Scoped** lifetimes instead.

---

## 🛠️ Thread-Safe Implementation

```csharp
public sealed class MutableSingletonCache
{
    // Lazy<T> ensures thread-safe, thread-locked initialization
    private static readonly Lazy<MutableSingletonCache> _instance = 
        new Lazy<MutableSingletonCache>(() => new MutableSingletonCache());

    // ConcurrentDictionary prevents memory and index corruption under concurrent writes
    private readonly ConcurrentDictionary<string, string> _featureFlags = new();

    private MutableSingletonCache() {}

    public static MutableSingletonCache Instance => _instance.Value;

    public void UpdateFlag(string key, string value)
    {
        _featureFlags[key] = value; 
    }

    public string GetFlag(string key)
    {
        return _featureFlags.TryGetValue(key, out var val) ? val : "disabled";
    }
}
```

