# 🔒 Concurrency Cheatsheet: .NET vs. Database Deadlocks

A deadlock occurs when two or more execution tracks block each other permanently while waiting for a resource. 

---

## 1. .NET Application Deadlock (Thread Pool Starvation)

> **The Analogy:** **A One-Lane Bridge.** Two cars drive toward each other and meet in the middle. Neither can move forward, and neither can back up. They are stuck forever.

### The Panel Trap
Synchronously blocking an asynchronous task using `.Result` or `.Wait()`.

```csharp
// ❌ THE DISASTER TRAP
public string GetDocumentJson(string id)
{
    var task = _blobService.FetchAsync(id);
    return task.Result; // 💥 Request thread freezes synchronously, causing global starvation.
}
```

### The Mechanic
The request thread blocks waiting on `.Result`. When the async task finishes, it tries to resume execution on that original thread context. Because that thread is blocked waiting for the task, the application freezes.

### The Fix
Maintain an asynchronous pipeline from top to bottom using `async` and `await`.

```csharp
//  CORRECT DEFENSE
public async Task<string> GetDocumentJsonAsync(string id)
{
    return await _blobService.FetchAsync(id);
}
```

If you are writing pure, synchronous code where you want everything to run line-by-line on a single thread, the synchronous methods below are completely fine to use.
The danger happens when you mix them—known as the "sync-over-async" anti-pattern which causes thread pool starvation.

### Cheat Sheet: Synchronous vs. Asynchronous Replacements

| Do Not Use (Synchronous / Blocking) | Use Instead (Asynchronous / Non-blocking) |
| :--- | :--- |
| `task.Result` / `task.Wait()` | `await task;` |
| `task.GetAwaiter().GetResult()` | `await task;` |
| `Task.WaitAll(t1, t2);` | `await Task.WhenAll(t1, t2);` |
| `Thread.Sleep(1000);` | `await Task.Delay(1000);` |
| `lock (locker) { ... }` | `await semaphore.WaitAsync(); try { ... } finally { semaphore.Release(); }` |
| `stream.Read(buffer, 0, len);` | `await stream.ReadAsync(buffer, 0, len);` |
| `query.ToList();` | `await query.ToListAsync();` |
| `query.ToArray();` | `await query.ToArrayAsync();` |
| `query.FirstOrDefault();` | `await query.FirstOrDefaultAsync();` |
| `query.Count();` | `await query.CountAsync();` |

---

## 2. Database Storage Deadlock (SQL Engine Page Locks)

> **The Analogy:** **The Secret Key Swap.** Lawyer A has the key to Vault 1 and wants the key to Vault 2. Lawyer B has the key to Vault 2 and wants the key to Vault 1. Both lock themselves inside and refuse to exchange keys.

### The Panel Trap
Concurrent transactions updating identical tables in an inconsistent structural sequence.

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
The SQL engine background Deadlock Detector spots the cyclic locking loop, kills one transaction as the **"Deadlock Victim"**, rolls back its changes, and throws **SQL Error 1205** to allow the other query to complete.

### The Fix
Enforce an identical database access order across all code routes, and handle transient rollbacks using retry blocks.

```csharp
//  CORRECT DEFENSE (Consistent Ordering + Poly Retry Engine)
public async Task ExecuteTransactionAsync(int lawyerId, int matterId)
{
    // 1. Enforce strict sequential order: Always update Lawyers table BEFORE Matters table
    await _context.Database.ExecuteSqlRawAsync("UPDATE Lawyers SET Status = 'Active' WHERE LawyerId = {0}", lawyerId);
    await _context.Database.ExecuteSqlRawAsync("UPDATE Matters SET Status = 'Active' WHERE MatterId = {0}", matterId);
}
```
