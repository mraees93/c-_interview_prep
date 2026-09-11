# C# Intermediate Async & Performance Cheatsheet

### 🚨 Async Loop Execution Strategies

#### ❌ The Sequential Bottleneck (Bad Practice)
Awaiting directly inside a loop forces tasks to run one after another, destroying performance.
```csharp
// Total time: 5 items * 1000ms = 5 Seconds
foreach (var id in ids)
{
    var data = await FetchDataAsync(id); // Pauses thread pool loop execution
    results.Add(data);
}
```

#### 🟢 The Concurrent Batch (Best Practice for Independent Tasks)
Fire all tasks into the background concurrently and await the batch at the end.
```csharp
// Total time: Longest single task = 1 Second
var tasks = ids.Select(id => FetchDataAsync(id)).ToList();
var results = await Task.WhenAll(tasks);
```

#### 🔵 The Sequential Workflow (Acceptable Use of Awaiting Inside Loops)
Awaiting inside a loop is perfectly valid **only when Task B strictly depends on the successful output or outcome of Task A**.
```csharp
// Use this pattern when strict execution sequence or pagination is required
foreach (var pageUrl in paginatedUrls)
{
    var pageData = await DownloadPageAsync(pageUrl); // Must finish completely before we can extract the 'NextPageUrl' pointer
    if (pageData.NextPageUrl == null) break;
}
```

#### 🟡 The Throttled Approach (Enterprise Best Practice for Mass Data)
Use `SemaphoreSlim` to run tasks concurrently without overwhelming SQL connection pools or external API rate limits.
```csharp
// Processes items concurrently, but limits active executions to 3 at a time
using var semaphore = new SemaphoreSlim(3);
var tasks = ids.Select(async id =>
{
    await semaphore.WaitAsync();
    try { return await FetchDataAsync(id); }
    finally { semaphore.Release(); }
});
await Task.WhenAll(tasks);
```

---

### 🗂️ Async Core Rules of Thumb

| Requirement | Correct Pattern | Avoid / Anti-Pattern | Reason |
| :--- | :--- | :--- | :--- |
| **API Entry Point** | `async Task` | `async void` | `async void` crashes the entire web host process if an exception occurs. |
| **Awaiting a Task** | `await MyMethodAsync()` | `task.Wait()` / `.Result` | Blocking sync blocks cause thread-pool starvation and deadlocks. |
| **High-Throughput Sync Path** | `ValueTask<T>` | `Task<T>` | `ValueTask` avoids heap memory allocation when data is retrieved from local cache. |
| **Shared Code Libraries** | `await task.ConfigureAwait(false)` | Standard `await` | Prevents capturing UI/Sync context unnecessarily, boosting performance. |

---

### 💥 Catching Concurrent Exceptions (`Task.WhenAll`)

Standard `catch (Exception ex)` only catches the **first** exception that faults. Use the captured task instance to unpack the complete `AggregateException` payload.

```csharp
var taskA = FaultyTaskAAsync();
var taskB = FaultyTaskBAsync();
var combinedTasks = Task.WhenAll(taskA, taskB);

try
{
    await combinedTasks;
}
catch (Exception)
{
    if (combinedTasks.Exception != null)
    {
        foreach (var innerEx in combinedTasks.Exception.InnerExceptions)
        {
            Console.WriteLine(\$"Logged Error: {innerEx.Message}");
        }
    }
}
```
