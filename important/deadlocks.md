# 🔒 Concurrency Cheatsheet: App Starvation, Code Deadlocks, & SQL Locks

This cheatsheet outlines the three critical concurrency threats that can freeze an enterprise application layer.

---

## 1. App Concurrency Threat A: Thread Pool Starvation (Sync-Over-Async)

> **The Analogy:** **The Broken Drive-Thru.** A chef starts a slow task and stands completely frozen, staring at the oven door. Because all the chefs in the kitchen are frozen staring at oven doors, there is no one left in the backroom to pick up completed meals or take new customer orders. The entire restaurant grinds to a halt.

### The Panel Trap
Synchronously blocking an asynchronous task using `.Result` or `.Wait()` inside a web request loop.

```csharp
// ❌ THE DISASTER TRAP
public string GetDocumentJson(string id)
{
    var task = _blobService.FetchAsync(id);
    return task.Result; // 💥 Request thread freezes synchronously, causing system-wide starvation.
}
```

### The Mechanic
When a thread hits `.Result`, it blocks itself synchronously. When the underlying async task finishes, it posts a message to the **Thread Pool** backroom asking for a free worker to resume the code. If high traffic causes multiple endpoints to call `.Result` simultaneously, **every worker in the thread pool becomes blocked**. The app runs out of threads, cannot process the completion messages, and starves.

### The Fix
Maintain an asynchronous pipeline from top to bottom using `async` and `await` so threads can safely return to the pool while waiting.

```csharp
//  CORRECT DEFENSE
public async Task<string> GetDocumentJsonAsync(string id)
{
    return await _blobService.FetchAsync(id);
}
```

### Cheat Sheet: Synchronous vs. Asynchronous Replacements

| Do Not Use (Synchronous / Blocking) | Use Instead (Asynchronous / Non-blocking) |
| :--- | :--- |
| `task.Result` / `task.Wait()` | `await task;` |
| `task.GetAwaiter().GetResult()` | `await task;` |
| `Task.WaitAll(t1, t2);` | `await Task.WhenAll(t1, t2);` |
| `Thread.Sleep(1000);` | `await Task.Delay(1000);` |
| `lock (locker) { ... }` | `await semaphore.WaitAsync(); try { ... } finally { semaphore.Release(); }` |
| `query.ToList();` | `await query.ToListAsync();` |

---

## 2. App Concurrency Threat B: Classic Code Deadlocks (Nested Mutex Locks)

> **The Analogy:** **The Two Stubborn Chefs.** Chef A grabs the **Main Stove** and refuses to let go until they get the **Cutting Board**. At the exact same microsecond, Chef B grabs the **Cutting Board** and refuses to let go until they get the **Main Stove**. Both chefs stand frozen in the middle of the kitchen forever, waiting for the other to drop their tool.

### The Panel Trap
Two different threads acquire locks on the exact same two shared objects on the **Warehouse Floor (the Heap)**, but they grab them in **opposite orders**. 

```csharp
// Shared equipment out on the Warehouse Floor (Heap)
private readonly object _stoveLock = new();
private readonly object _boardLock = new();

// ❌ THE DEADLOCK TRAP (Opposite Grab Orders)

// Chef A (Thread 1) runs this method:
public void PrepareSteak()
{
    lock (_stoveLock) // 1. Chef A locks the Stove
    {
        Thread.Sleep(50); // Simulates prepping meat
        lock (_boardLock) // 2. 💥 FREEZES! Waits for Chef B to drop the Board
        {
            Console.WriteLine("Steak plated.");
        }
    }
}

// Chef B (Thread 2) runs this method at the exact same time:
public void PrepareSalad()
{
    lock (_boardLock) // 1. Chef B locks the Cutting Board
    {
        Thread.Sleep(50); // Simulates chopping veggies
        lock (_stoveLock) // 2. 💥 FREEZES! Waits for Chef A to drop the Stove
        {
            Console.WriteLine("Salad plated.");
        }
    }
}
```

### The Fix
You eliminate deadlocks through strict kitchen rules rather than adding more locks:

1. **Enforce Strict Lock Ordering:** Force every chef in the kitchen to grab equipment in the exact same sequence (e.g., **Always grab the Stove BEFORE the Cutting Board**). If Chef B has to wait for the Stove first, Chef A can easily grab the Cutting Board, finish the steak, and drop both tools.
2. **Use Async Timeout Gates:** Replace rigid `lock` blocks with `SemaphoreSlim.WaitAsync(timeout)` so a chef automatically gives up and steps back if a tool isn't released within 2 seconds.

```csharp
//  THE CURE: Strict Lock Ordering + Async Timeout Gate
private readonly SemaphoreSlim _stoveGate = new(1, 1);
private readonly SemaphoreSlim _boardGate = new(1, 1);

public async Task PrepareSaladSafelyAsync()
{
    // 1. Follow the exact same order as PrepareSteak: Stove FIRST, Board SECOND
    if (await _stoveGate.WaitAsync(TimeSpan.FromSeconds(2)))
    {
        try
        {
            if (await _boardGate.WaitAsync(TimeSpan.FromSeconds(2)))
            {
                try
                {
                    Console.WriteLine("Salad plated safely with zero deadlock risk!");
                }
                finally { _boardGate.Release(); }
            }
        }
        finally { _stoveGate.Release(); }
    }
}
```

---

## 3. Database Storage Deadlock (SQL Engine Page Locks)

> **The Analogy:** **The Secret Key Swap.** Lawyer A has the key to Vault 1 and wants the key to Vault 2. Lawyer B has the key to Vault 2 and wants the key to Vault 1. Both lock themselves inside and refuse to exchange keys.

### The Panel Trap
Concurrent database transactions updating identical tables or rows in an inconsistent, unaligned structural sequence.

```text
TRANSACTION 1 (Thread A)                 TRANSACTION 2 (Thread B)
────────────────────────                 ────────────────────────
1. UPDATE Lawyers SET Status = 'Active'  1. UPDATE Matters SET Status = 'Closed'
   WHERE LawyerId = 10;                     WHERE MatterId = 500;
   -- Locks row in Lawyers                  -- Locks row in Matters

2. UPDATE Matters SET Status = 'Active'  2. UPDATE Lawyers SET Status = 'Active'
   WHERE MatterId = 500;                    WHERE LawyerId = 10;
   -- 💥 BLOCKS: Waiting for Trans 2        -- 💥 BLOCKS: Cyclic loop detected!
```

### The Mechanic
When queries lack selective indexes, the SQL engine escalates row locks into massive page or table locks. If two unaligned transactions intersect, they trap each other. The SQL background Deadlock Detector spots the cyclic loop, kills one transaction as the **"Deadlock Victim"**, rolls back its changes, and throws **SQL Error 1205** to let the other complete.

### The Fix
1. **Pinpoint Indexing:** Prevent table scans by indexing condition columns (like `LawyerId`), forcing the engine to use isolated row-level locks.
2. **Consistent Database Ordering:** Enforce an identical table modification sequence across all backend code routes.

```csharp
//  CORRECT DEFENSE (Consistent Ordering + SQL Row Locks)
public async Task ExecuteTransactionAsync(int lawyerId, int matterId)
{
    // Enforce strict sequential order: Always update Lawyers table BEFORE Matters table
    await _context.Database.ExecuteSqlRawAsync("UPDATE Lawyers SET Status = 'Active' WHERE LawyerId = {0}", lawyerId);
    await _context.Database.ExecuteSqlRawAsync("UPDATE Matters SET Status = 'Active' WHERE MatterId = {0}", matterId);
}
```
