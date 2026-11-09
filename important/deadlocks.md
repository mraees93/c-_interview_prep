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

> **The Analogy:** **A One-Lane Bridge.** Two cars drive toward each other from opposite sides and meet exactly in the middle. Neither can move forward, and neither can back up. They are locked in a permanent standoff.

### The Panel Trap
Acquiring multiple software locks (`lock` or `SemaphoreSlim`) in an inconsistent, alternating order across different threads.

```csharp
// Thread A execution path:              // Thread B execution path:
lock (LockKitchenRange)                  lock (LockCuttingBoard)
{                                        {
    // Holds Range, waits for Board          // Holds Board, waits for Range
    lock (LockCuttingBoard) { ... }          lock (LockKitchenRange) { ... }
}                                        }
// 💥 DEADLOCK: Thread A and Thread B freeze each other permanently.
```

### The Fix
1. **Enforce strict lock ordering:** Always lock resource assets in the exact same structural or alphabetical sequence everywhere in the app.
2. **Use Timeout Gates:** Use `semaphore.WaitAsync(TimeSpan.FromSeconds(2))` so a thread automatically abandons the wait if a deadlock standoff occurs.

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
