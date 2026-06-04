# ⚖️ C# System Rules vs. TypeScript Flexibility
*LexisNexis Cape Town Interview Preparation - Module 5*

## 🥊 Structural Rule Head-to-Head

Understanding why C# rejects common TypeScript habits is the key to passing an intermediate backend architecture review.


| Rule Concept | JavaScript / TypeScript | C# (.NET Core Runtime) |
| :--- | :--- | :--- |
| **Bypassing Types** | `any` / `unknown` (Completely disables type checks) | `dynamic` / `object` (Retains runtime overhead and type tracing) |
| **Object Extension** | `obj.newProp = 123;` (Objects are freely dynamic) | **Forbidden** (Objects are locked to their strict Class/Struct definitions) |
| **Top-Level Code** | Functions and variables can exist globally | Everything must be explicitly wrapped in a Class or Struct namespace |
| **Array Sizing** | Arrays scale and grow dynamically in memory | Arrays are fixed-size chunks of memory upon creation |
| **Missing Values** | Has both `null` and `undefined` | Only has `null` (Uninitialized types have implicit defaults) |

---

## 🚨 The Definitive C# "Gotchas" Library for TS Developers

### 1. The Variable Shadowing Trap
In TypeScript, you can declare an inner variable that has the same name as an outer variable. In C#, this triggers an immediate compiler error because child block scopes cannot override parent block variable registers.

```csharp
public class ScopeDemo
{
    public void ProcessData()
    {
        int recordId = 101;

        if (recordId > 100)
        {
            // ❌ COMPILE ERROR: A local variable named 'recordId' cannot be declared in this scope 
            // because it would give a different meaning to 'recordId', which is already used.
            int recordId = 505; 
            Console.WriteLine(recordId);
        }
    }
}
```

### 2. Array Immutability vs. List Flexibility
```csharp
using System;
using System.Collections.Generic;

public class ArrayRuleDemo
{
    public static void Main()
    {
        // ❌ THE FIX-SIZE TRAP: Looks like a JS array, but memory allocation is permanently locked to 2 slots.
        string[] staticEmployees = new string { "Alice", "Bob" };
        // staticEmployees = "Charlie"; // 💥 Throws an IndexOutOfRangeException at runtime!

        //  THE PRODUCTION SOLUTION: Use List<T> for standard dynamic JavaScript array behavior
        List<string> dynamicEmployees = new List<string> { "Alice", "Bob" };
        dynamicEmployees.Add("Charlie"); // Completely safe. Internal buffer grows automatically.
    }
}
```

### 3. The Object/Reference Equality Trap
In JS, comparing objects (`{} === {}`) compares reference memory addresses, but comparing primitives (`"abc" === "abc"`) checks values. In C#, `==` behavior depends strictly on whether the underlying type is a `class`, a `struct`, or a `record`.

```csharp
using System;

public class UserClass { public string Name { get; set; } }
public record UserRecord { public string Name { get; set; } }

public class EqualityDemo
{
    public static void CheckEquality()
    {
        UserClass u1 = new UserClass { Name = "Thabo" };
        UserClass u2 = new UserClass { Name = "Thabo" };
        // ❌ FALSE: They hold identical data, but they point to different Heap memory addresses.
        bool classEq = (u1 == u2); 

        UserRecord r1 = new UserRecord { Name = "Thabo" };
        UserRecord r2 = new UserRecord { Name = "Thabo" };
        //  TRUE: C# 'record' types automatically implement value-based equality.
        bool recordEq = (r1 == r2); 
    }
}
```

### 4. The Silent Integer Division Trap
In JavaScript, all numbers are floats (`64-bit binary float`). `5 / 2` yields `2.5`. In C#, dividing two integers always drops the fraction and returns an integer. 

```csharp
public class DivisionDemo
{
    public static void Calculate()
    {
        int a = 5;
        int b = 2;
        
        // ❌ TRAP: result is EXACTLY 2.0! The mathematical truncation happens BEFORE conversion to double.
        double result = a / b; 

        //  THE FIX: Force at least one operand to be a float/double literal
        double fixedResult = (double)a / b; // Returns 2.5
    }
}
```

### 5. `undefined` is Missing: Default Value Struct Mechanics
JavaScript uses `undefined` for unassigned fields. C# doesn't have `undefined`. If you declare a value-type field (like `int`, `bool`, `DateTime`) inside a class and don't assign it a value, C# automatically gives it an implicit default memory value (`0`, `false`, or `01/01/0001`).

```csharp
public class Registration
{
    public int Age { get; set; } // Defaults to 0 automatically. Never null!
    public bool IsVerified { get; set; } // Defaults to false automatically.
    
    //  THE FIX: Use Nullable types (T?) if a property must support missing states
    public int? OptionalScore { get; set; } // Can be null, mimicking TS 'number | null'
}
```

### 6. The Deceptive LINQ `FirstOrDefault` Object Crash
When using JS `.find()`, if no element matches, it returns `undefined`, allowing you to write a clean optional chain (`result?.id`). In C#, `.FirstOrDefault()` on a struct collection (like `int`) returns the struct's default value (`0`), which can break logic checks. On classes, it returns `null` which throws an immediate exception if not verified safely.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class LinqCrashDemo
{
    public static void Process()
    {
        var numbers = new List<int> { 5, 10, 15 };
        
        // ❌ TRAP: No number is greater than 50. FirstOrDefault() returns 0!
        int match = numbers.FirstOrDefault(x => x > 50); 
        if (match == 0) 
        {
            // You cannot easily determine if '0' was an actual array value or the failure fallback!
        }
    }
}
```

---

## 🛠️ Explicit Method Parameter Rules (`ref` and `out`)
In TypeScript, primitive types (numbers, booleans, strings) are strictly passed by value, and you cannot alter their original memory state from inside a child function. C# provides special keywords (`ref` and `out`) to allow methods to modify the original variable reference directly.

```csharp
public class ParameterRuleDemo
{
    // The 'out' keyword guarantees that this method MUST assign a value to 'result' before exiting.
    public void CalculateMetrics(int input, out int result)
    {
        result = input * 10; // Required assignment
    }

    public void Run()
    {
        int finalOutput; // Variable is uninitialized
        
        // Passing via 'out' populates the uninitialized local variable directly
        CalculateMetrics(5, out finalOutput);
        
        System.Console.WriteLine(finalOutput); // Outputs 50
    }
}
```
Speak in Native C# Terminology: In the interview, use native .NET vocabulary instead of the TypeScript equivalent.Say "List", not "Dynamic Array".Say "Dictionary", not "Map or Key-Value Object".Say "Task", not "Promise".Say "LINQ Projection", not ".map method".