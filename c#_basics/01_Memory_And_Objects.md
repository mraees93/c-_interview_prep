# 🏛️ C# Memory Architecture & Object Mappings
*LexisNexis Cape Town Interview Preparation - Module 1*

## 🧵 Memory Architecture: String vs. StringBuilder

### Concept Summary
*   **`System.String` (Immutable):** Every modification creates a brand-new object on the managed heap. Doing this inside loops leads to **heap fragmentation** and triggers heavy **Garbage Collection (GC) spikes**, destroying application throughput.
*   **`System.Text.StringBuilder` (Mutable):** Allocates a resizable internal character buffer heap space. It modifies memory in-place, making it the required choice for linear or loop-based string manipulations.

### JS/TS Mental Model Map
*   **JS Strings are also immutable.** However, modern JavaScript engines (like V8) automatically optimize basic loop concatenations (`str += i`) behind the scenes using internal structures like "Ropes". 
*   **C# CLR does not do this automatically.** You must manually optimize string operations using `StringBuilder`.

### 🚨 Common Problem: Diagnostic Log Concatenation
```csharp
using System;
using System.Text;

public class StringPerfDemo 
{
    // ❌ INTERVIEW RED FLAG: Allocates 5,000 distinct string objects on the Heap
    public static string BadLogProcessor(string[] events) 
    {
        string result = string.Empty;
        foreach (var ev in events) 
        {
            result += \$"[LOG]: {ev}\n"; 
        }
        return result;
    }

    //  INTERVIEW GREEN FLAG: Zero unnecessary heap allocations
    public static string GoodLogProcessor(string[] events) 
    {
        // Pre-size the internal array buffer if total size is roughly predictable
        var sb = new StringBuilder(events.Length * 30); 
        
        foreach (var ev in events) 
        {
            sb.Append("[LOG]: ").Append(ev).Append("\n");
        }
        return sb.ToString(); // Single final heap allocation
    }
}
```

---

## 📦 Object Representation: JavaScript/TypeScript vs. C#

In JavaScript, objects (`{}`) serve as records, dictionaries, and dynamic shapes. C# separates these roles into highly specialized data structures to guarantee type safety and optimal memory layouts.

### 1. The Dynamic Dictionary Lookup Table
*   **JS/TS:** `const map = new Map<string, number>();` or object literals used as hash maps.
*   **C#:** `Dictionary<TKey, TValue>`. Statically typed, fast key lookups via internal bucket arrays.

#### 🚨 INTERVIEW TRAP: Accessing Dictionary Data
In JavaScript/TypeScript, you can read properties dynamically via Dot Notation (`recordScores.User_A`). In C#, **Dot Notation is strictly forbidden** on Dictionaries because it is a type collection, not a structural dynamic object wrapper. You must use indexers or safe retrieval methods.

```csharp
using System;
using System.Collections.Generic;

public class DictionaryAccessDemo
{
    public static void Run()
    {
        var recordScores = new Dictionary<string, int> 
        {
            { "User_A", 95 },
            { "User_B", 88 }
        };

        // ❌ COMPILE ERROR: Dot notation does not look inside the map keys
        // int badCall = recordScores.User_A; 

        // ⚠️ INDEPENDENT INDEXER NOTATION: Equivalent to JS bracket notation recordScores["User_A"]
        // CRITICAL TRAP: If the key does not exist, this throws a KeyNotFoundException and crashes your server!
        int score = recordScores["User_A"]; 

        //  PRODUCTION-SAFE .NET APPROACH (TryGetValue)
        // Checks for existence and assigns the variable inline using an 'out' parameter without crashing
        if (recordScores.TryGetValue("User_C", out int userScore))
        {
            Console.WriteLine(\$"Score found: {userScore}");
        }
        else
        {
            Console.WriteLine("Key not found safely without a runtime exception.");
        }
    }
}
```

### 2. Ad-hoc Local Data Shapes (Anonymous Types)
*   **JS/TS:** Returning an un-typed immediate object literal: `return { id: 5, action: "process" };`
*   **C#:** **Anonymous Types**. Created inline, read-only properties, and structural identity inferred by the compiler. Excellent for intermediate projections inside LINQ statements.

```csharp
// Read-only object wrapper created on the fly
var tempPayload = new { Id = 101, Status = "CapeTown_Active" };
// tempPayload.Id = 202; // ❌ Compile Error: Anonymous type properties are strictly immutable.
```

### 3. Fully Dynamic Runtime Objects
*   **JS/TS:** Native behaviour of vanilla JavaScript objects where keys can be appended or mutated at runtime.
*   **C#:** `ExpandoObject` paired with the `dynamic` keyword. Bypasses the compiler's type checking system completely.

```csharp
using System;
using System.Dynamic;

dynamic dynamicRecord = new ExpandoObject();
dynamicRecord.Id = 456;
dynamicRecord.UpdateStatus = (Action<string>)((status) => Console.WriteLine(\$"Status set to: {status}"));

// Invoked identically to a JavaScript closure
dynamicRecord.UpdateStatus("Approved");
```

---

## 💾 Intermediate Bonus Topic: Value Types (Structs) vs. Reference Types (Classes)

Interviewers will evaluate your deep awareness of memory allocation patterns.

*   **Classes (Reference Types):** Allocated on the Managed Heap. Variables hold a reference pointer to the actual data address [1]. Passing a class object passes the reference pointer [1].
*   **Structs (Value Types):** Allocated inline on the Stack [1]. Variables hold the direct actual values [1]. Passing a struct copies the **entire internal values** to the new context memory block [1].

### 🚨 Common Problem: Accidental Value Mutation Bugs
```csharp
using System;

public struct PointStruct { public int X; public int Y; }
public class PointClass { public int X; public int Y; }

public class MemoryDemo
{
    public static void MutateData()
    {
        PointClass c1 = new PointClass { X = 10 };
        PointClass c2 = c1; // Passes reference pointer [1]
        c2.X = 99; // Alters c1.X as well! Both look at the same heap address.

        PointStruct s1 = new PointStruct { X = 10 };
        PointStruct s2 = s1; // Copies the ENTIRE actual values to a new stack position [1]
        s2.X = 99; // s1.X stays EXACTLY 10! Structural isolation occurs.
    }
}
```
