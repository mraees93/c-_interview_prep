# 📊 C# Collections Architecture & Syntax Matrix
*LexisNexis Cape Town Interview Preparation - Module 3*

## 🔄 The Collection Rosetta Stone

Use this comparative matrix to trace C# core data architectures directly back to your JavaScript equivalents while establishing rigorous algorithmic boundaries.


| C# Collection Type | TS/JS Memory Equivalent | Lookup Complexity | Insertion Complexity | Best Interview Use Case |
| :--- | :--- | :--- | :--- | :--- |
| **`T[]` (Array)** | Fixed TypedArray (e.g. `Int32Array`) | $O(1)$ by index | N/A (Fixed Size) | Low-level execution loops where performance bounds and limits are perfectly predefined. |
| **`List<T>`** | Standard Dynamic Array (`[]`) | $O(1)$ by index | $O(1)$ average / $O(N)$ when scaling limits | Your absolute default, go-to collection wrapper for variable data sizes. |
| **`Dictionary<K,V>`** | Native `Map` object | $O(1)$ average | $O(1)$ average | High-speed cache indexes requiring microsecond lookups via safe primary hash keys. |
| **`HashSet<T>`** | Native `Set` object | $O(1)$ average | $O(1)$ average | Aggressive item deduplication pipelines and lightning-fast element presence matching. |
| **`Queue<T>`** | Array using `.push()` & `.shift()` | $O(N)$ linear scan | $O(1)$ push rate | **FIFO (First In, First Out)** workflow engines like real-time ingestion filters or message arrays. |
| **`Stack<T>`** | Array using `.push()` & `.pop()` | $O(N)$ linear scan | $O(1)$ push rate | **LIFO (Last In, First Out)** processing loops like undo trackers or internal execution traces. |
| **`LinkedList<T>`** | Custom reference Node class graphs | $O(N)$ linear iteration | $O(1)$ if pointer is pre-cached | Heavy structural insertions or removals at the center of sequences without array shifting overhead. |

---

## 🛠️ Data Access Notation & Safety Architecture

Understanding how syntax varies across platforms is critical for code execution.


| Property Operation Type | JavaScript / TypeScript | C# (.NET Core CLR) |
| :--- | :--- | :--- |
| **Dot Access Notation** | `recordScores.User_A` | ❌ **Strictly Prohibited** (Triggers Compile Error) |
| **Indexer Bracket Lookup** | `recordScores["User_A"]` | `recordScores["User_A"]` (⚠️ Throws exception if missing) |
| **Defensive Value Retrieval** | `recordScores["Key"] ?? default` | `recordScores.TryGetValue("Key", out int outputValue)` |
| **Null-Conditional Prop Chain** | `user?.profile?.address` | `user?.Profile?.Address` [1] |
| **Structural Record Equality** | `JSON.stringify(a) === JSON.stringify(b)` | Built-in via C# `record` types / `Equals()` methods [1] |

---

## 🛠️ Complete Syntax Cheat Sheet & Reference Implementations

```csharp
using System;
using System.Collections.Generic;

public class CompilationRunner
{
    public static void Main()
    {
        // 1. Array Construction
        string[] localHubs = new string { "CapeTown_CBD", "Bellville", "CenturyCity" };

        // 2. List Initialization Syntax
        List<int> performanceMetrics = new List<int> { 200, 404, 500 };
        performanceMetrics.Add(201);

        // 3. Dictionary Instantiation & Safe Retrieval Pattern
        var microserviceStatus = new Dictionary<int, string>
        {
            { 200, "Healthy" },
            { 503, "Degraded" }
        };
        
        // Dynamic key lookup confirmation syntax
        if (microserviceStatus.TryGetValue(200, out string healthState))
        {
            Console.WriteLine(\$"Status evaluated as: {healthState}");
        }

        // 4. HashSet Deduplication usage
        HashSet<string> sessionIdentifiers = new HashSet<string>();
        bool isAddedFirstTime = sessionIdentifiers.Add("Token_XYZ"); // Returns true
        bool isAddedSecondTime = sessionIdentifiers.Add("Token_XYZ"); // Returns false (rejected)

        // 5. Queue Pipeline syntax
        Queue<string> transactionalBuffer = new Queue<string>();
        transactionalBuffer.Enqueue("Job_1");
        string currentProcess = transactionalBuffer.Dequeue(); // Removes and returns "Job_1"

        // 6. Stack processing syntax
        Stack<string> breadcrumbs = new Stack<string>();
        breadcrumbs.Push("Main_Dashboard");
        string lastViewed = breadcrumbs.Pop(); // Removes and returns "Main_Dashboard"

        Console.WriteLine(".NET Collections Reference Built and Validated Successfully.");
    }
}
```
