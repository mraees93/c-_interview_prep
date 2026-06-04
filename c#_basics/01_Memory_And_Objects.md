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

```csharp
using System.Collections.Generic;

var recordScores = new Dictionary<string, int> 
{
    { "User_A", 95 },
    { "User_B", 88 }
};
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
