# C# Basics & .NET Runtime Mechanics - Interview Preparation

This module tracks core C# compiler rules, assembly scoping, and memory/performance characteristics essential for high-volume enterprise document and data pipelines.

---

## 1. Assembly Boundaries & Access Modifiers (The Structural Challenge)

### The Panel Scenario
Large platforms use multi-project solutions. Interviewers test your deep understanding of scoping boundaries across distinct compiled assemblies to see if you can protect infrastructure systems from public access.

Consider two assemblies in a solution:
1. `LexisNexis.Core.Documents` (Base engine)
2. `LexisNexis.Search.Engine` (Search engine referencing Core)

```csharp
// Inside Assembly 1: LexisNexis.Core.Documents
namespace LexisNexis.Core.Documents
{
    public class LegalBrief
    {
        protected internal string CaseId { get; set; }     // Property A
        private protected string DocumentHash { get; set; }  // Property B
        internal string StoragePath { get; set; }           // Property C
    }

    public class SubclassInSameAssembly : LegalBrief
    {
        public void TestAccess() { /* Q1: Access check */ }
    }
}

// Inside Assembly 2: LexisNexis.Search.Engine
using LexisNexis.Core.Documents;
namespace LexisNexis.Search.Engine
{
    public class SubclassInDifferentAssembly : LegalBrief
    {
        public void TestAccess() { /* Q2: Access check */ }
    }

    public class IndependentClass
    {
        public void TestAccess() { /* Q3: Access check */ }
    }
}
```

### Questions & Core Answers
*   **Q1: Which properties can `SubclassInSameAssembly` access?**
    *   **Answer**: Properties **A, B, and C**. Since it resides in the same assembly, `internal` is valid. `private protected` allows access within the same assembly for subclasses, and `protected internal` allows access within the same assembly OR via inheritance.
*   **Q2: Which properties can `SubclassInDifferentAssembly` access?**
    *   **Answer**: Property **A** only. `protected internal` permits cross-assembly access *exclusively* via inheritance. Both `private protected` and `internal` are strictly bound to their original assembly.
*   **Q3: Which properties can `IndependentClass` access?**
    *   **Answer**: **None**. It lacks both assembly membership and an inheritance relationship, leaving it completely locked out.

---

## 2. Reference Types vs. Value Types (Memory Allocation)

### The Panel Question
What is the structural difference between a `struct` and a `class` in C#, and how do they impact the .NET Garbage Collector?

### Core Answer
*   **Allocation**: `struct` is a value type typically allocated on the **Stack**. `class` is a reference type allocated on the **Managed Heap**.
*   **GC Impact**: High-frequency allocation of classes triggers Garbage Collection (GC) sweeps to reclaim heap memory, creating micro-pauses. Value types on the stack are instantly deallocated when their containing method completes, bypassing the GC entirely. 

---

## 3. Data Entities: Class vs. Record (Memory & Equality)

### The Panel Scenario
When parsing gigabytes of legal records, choosing the right definition avoids object bloat and memory fragmentation.

```csharp
public class SearchMatchClass {
    public string DocumentId { get; set; }
    public int Position { get; set; }
}

public record SearchMatchRecord(string DocumentId, int Position);
```

### Questions & Core Answers
*   **Q1: How do they differ regarding mutability and equality checks by default?**
    *   **Answer**: A standard `class` is mutable by default and uses **Reference Equality** (compares memory addresses). A positional `record` is immutable by default (uses `init`-only properties) and implements **Value Equality** (compares inner data values).
*   **Q2: If two separate instances have identical data, how does `==` behave?**
    *   **Answer**: For the class, `class1 == class2` returns `false` because they sit at different memory locations. For the record, `record1 == record2` returns `true` because the compiler overrides the equality operators to check property data values directly.

---

## 4. Async/Await and Threading Mechanics

### The Panel Question
What happens when you invoke an asynchronous method without utilizing the `await` keyword, and what is a `Deadlock` in legacy .NET contexts?

### Core Answer
*   **Missing Await**: The compiler triggers a warning, and the code executes as a **fire-and-forget** operation. The thread continues executing the next line immediately without waiting for the task to finish, leading to race conditions or unhandled background failures.
*   **Deadlocks**: This occurs when a thread blocks synchronously on an async task (e.g., calling `.Result` or `.Wait()`) while the async task tries to marshal execution back to that same blocked thread context via the `SynchronizationContext`.


## 5. String Optimization: String vs. StringBuilder

### The Panel Question
Why is modifying a raw `string` inside a large loop considered a performance anti-pattern, and how does `StringBuilder` fix it?

### Core Answer
*   **The Problem**: Strings in C# are **immutable** (unchangeable). Every time you perform an operation like `str += "new text"`, the .NET runtime does not append the text. Instead, it allocates an entirely new string object on the Managed Heap and leaves the old one behind, causing severe memory fragmentation and triggering the Garbage Collector.
*   **The Fix**: `StringBuilder` allocates an internal, mutable expandable character array buffer on creation. It modifies this buffer directly in-place without generating thousands of temporary heap objects, making it drastically faster and memory-efficient for loops or heavy string manipulation.

---

## 6. String Interning Engine

### The Panel Question
What is String Interning in .NET, and how does it optimize memory?

### Core Answer
*   **The Engine**: The .NET CLR maintains an internal lookup table called the **Intern Pool**. When the application loads, the runtime stores exactly one instance of each unique literal string defined in your source code into this pool.
*   **The Optimization**: If you reference the exact same string literal across 10,000 different variables, the runtime points all 10,000 reference pointers to the single memory address inside the Intern Pool, preventing duplicate string allocations on the heap.

---

## 7. Deferred Execution via LINQ (`IEnumerable` vs `List`)

### The Panel Question
What is the danger of returning an unexecuted `IEnumerable<T>` from a repository, and what does "Deferred Execution" mean?

### Core Answer
*   **Deferred Execution**: LINQ queries built on an `IEnumerable` do not fetch data when they are declared. The execution is delayed until you actively loop over the collection (e.g., via a `foreach` loop or calling `.ToList()`).
*   **The Danger**: If you iterate over that `IEnumerable` three separate times in your business logic, the underlying database query will execute **three separate times** against your SQL database. To prevent this performance hit, materialise the data safely onto the heap using `.ToList()` or `.ToArray()` before processing it.
