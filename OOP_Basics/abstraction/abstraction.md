# C# Properties & Encapsulation Cheatsheet

# C# Abstraction Blueprint

## 🏛️ The Core Concept
Abstraction simply means hiding complex implementation details behind a simpler, cleaner interface. In C#, this is achieved across four distinct levels:

1. **Contract Abstraction (Interfaces & Abstract Classes):** Designing standard connection sockets so components can switch behaviors without breaking the system.
2. **Data Abstraction (Properties):** Wrapping a security checkpoint around raw warehouse fields so data cannot be maliciously altered.
3. **Execution Abstraction (Methods):** Hiding long lines of sequential logic behind a single descriptive action like `Console.WriteLine()`.
4. **Functional Abstraction (Delegates & Callbacks):** Passing whole execution rules dynamically using order slips like `Action<T>` and `Func<T, Result>`.

---

## 🏛️ The Core Concept: Recipe Guardrails
In C#, **Properties** act as smart security checkpoints or guardrails for your fields. 
* **The Field:** A raw object sitting out on the **Warehouse Floor (Heap)**.
* **The Property:** The velvet rope and the security guard monitoring who can look at (`get`) or touch (`set`) that pallet. 

---

## 🛠️ The 4 Property Variations

### 1. Auto-Implemented Properties (The Standard Door)
* **Best For:** Simple data containers where you don't need any validation logic.
* **How it works:** The compiler automatically generates a hidden, private backing field on the Heap behind the scenes.
```csharp
public class Chef
{
    // Publicly readable and writable anywhere
    public string Name { get; set; } 

    // Publicly readable, but can only be modified inside this class
    public int Age { get; private set; } 
}
```

### 2. Full Properties with Backing Fields (The Guarded Gate)
* **Best For:** Enforcing business rules, validation, or modifying data before it gets saved.
* **How it works:** You write an explicit `private` field (the raw data) and a separate `public` property to manage access to it.
```csharp
public class Order
{
    private decimal _price; // Private Backing Field

    public decimal Price
    {
        get => _price;
        set
        {
            // Business rule validation constraint
            if (value < 0) throw new ArgumentException("Price cannot be negative!");
            _price = value;
        }
    }
}
```

### 3. Computed Properties (The On-the-Fly Chef)
* **Best For:** Combining existing data or calculating values dynamically without storing a new variable in memory.
* **How it works:** It features a `get` block but has no backing field at all. It recalculates the answer every single time it is called.
```csharp
public class Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }

    // Computed property using clean expression-bodied lambda syntax (=>)
    public double Area => Width * Height; 
}
```

### 4. Init-Only Properties (The Immutable Setup - C# 9+)
* **Best For:** Making objects safe and unchangeable (immutable) after their initial creation phase.
* **How it works:** Replaces `set` with `init`. The property can *only* be assigned a value inside a constructor or during an object initialization block. After that, it is locked down permanently.
```csharp
public class Product
{
    public string Sku { get; init; } // Can never be changed after creation!
}

// Usage:
var item = new Product { Sku = "PROD-123" }; 
// item.Sku = "PROD-456"; // ❌ COMPILE ERROR: Init-only property cannot be assigned here
```

---

## 📋 The Golden Rule of C# Fields vs. Properties

* **Private Data:** Always use a raw **Field** (`private string _name;`).
* **Public Data:** Always use a **Property**—either an **Auto-Property** (`{ get; set; }`) for simple data, or a **Full Property** when you need custom guardrail logic.

> ⚠️ **The Double-Backing Trap:** Never combine an explicit `private` field with an auto-property `{ get; set; }`. Doing so accidentally allocates **two separate backing fields** on the Warehouse Floor (the Heap)—your manual one and the hidden one the compiler automatically generates!

---

## 🛠️ Implementation Scenarios

### Scenario A: Simple Public Variables (No Validation Needed)
* **The Practice:** Write an auto-property directly. Do not declare a separate field. The compiler will secretly build a hidden backing field for you out on the Warehouse Floor (Heap).

```csharp
// 🟢 BEST PRACTICE: One clean line. No separate field declared.
public string ChefName { get; set; }
```

### Scenario B: Strict Validation Logic (Guardrails)
* **The Practice:** This is the *only* time you write a manual field. You declare an explicit `private` backing field, and then write a full property with a custom code body to control read and write access.

```csharp
// 🟢 BEST PRACTICE: Separate private field + full property with logic
private int _age; 

public int Age
{
    get => _age;
    set
    {
        if (value < 0) throw new ArgumentException("Age cannot be negative!");
        _age = value;
    }
}
```

---

## 📋 Interview Questions & Answers

### Q1: What is the difference between a Field and a Property in C#?
**Answer:** 
* A **Field** is a raw variable declared directly inside a class. It represents direct access to a memory slot on the **Warehouse Floor (Heap)**. Exposing fields publicly breaks encapsulation because any external code can alter them without validation. **general rule of thumb, fields should almost always be declared as private or protected.**
When is a non-private field okay?: public const and public static readonly
* A **Property** is an extension of a field that exposes accessors (`get` and `set`). It wraps around data like a method wrapper, allowing you to intercept, validate, or compute data safely while keeping the underlying field protected.

### Q2: Why can't we just use public fields instead of auto-properties?
**Answer:** 
Using public fields creates rigid, hard-to-maintain code. If you expose a public field and later decide you need to add validation logic (e.g., checking if an age is negative), you would have to convert that field into a property. 
Doing this breaks **binary compatibility**. Any external application or library that was compiled against your old public field will instantly crash and require a complete re-compile because fields and properties generate entirely different IL (Intermediate Language) code instructions under the hood. Auto-properties future-proof your architecture.

### Q3: What is a backing field, and does an auto-property have one?
**Answer:** 
A backing field is a `private` class-level variable that holds the actual value returned or modified by a full property. Yes, an auto-property *does* have a backing field, but it is entirely anonymous and generated automatically by the compiler at compile-time. You cannot reference or see this hidden backing field directly in your C# code code blocks.

### Q4: If an object is shared across threads, does wrapping a field in a Property make it thread-safe?

**Answer:** 
**It depends on the accessor type used (`set` vs. `init`):**

* **No, if you use a standard `set` or `private set`:** These properties remain **mutable** (changeable at runtime). If two threads or background tasks try to modify the property at the exact same microsecond, they will collide and corrupt the underlying backing field. To make mutable properties thread-safe, you must protect them manually using your thread-safety toolkit (like a standard `lock`, an atomic `Interlocked` operation, or a `SemaphoreSlim`).
* **Yes, if you use an `init` setter (C# 9+):** An `init` property enforces **immutability**. It can only be assigned a value during object creation. Once initialization is finished, the underlying backing field is permanently locked down like a `readonly` field. Because threads are physically prevented from changing the data, multiple threads can safely read it simultaneously with zero risk of race conditions or memory corruption.

### Q5: Can you mark a property accessor with a different access modifier than the property itself?
**Answer:** 
Yes, C# allows you to apply a more restrictive access modifier to *one* of the accessors (typically the setter). For example, a property can be declared as `public string Title { get; private set; }`. This makes the property globally readable across the entire application, but restricts mutation permissions to code running inside that specific class layout.

---

You can hover over this block and click the **"Copy"** button to grab this cheatsheet for your study folder!

Now that we have encapsulated properties down, what's next?
* Explore how **Dependency Injection** hooks these encapsulated properties together across different files?
* Look at how the **Garbage Collector** cleans up these objects when they leave the Heap?
