# C# Object Instantiation Order & Interview Trap Guide

Whenever a class is instantiated in C# via the `new` keyword, the .NET runtime coordinates memory allocation, field initialization, and constructor execution in a strict, predictable sequence.

---

## The Execution Order Chain

If you have a child class inheriting from a parent class, the .NET runtime processes the exact sequence from the ground up like this:

1. **Child** field initializers run.
2. **Parent** field initializers run.
3. **Parent** constructor body runs.
4. **Child** constructor body runs.

### Why does it happen in this order?
The runtime intentionally runs the child fields first to guarantee that every single field in the entire object has a stable, assigned value before any setup logic in a base constructor can potentially access or interact with them.

---

## Example 1: Explicit Console Application Log Trace

This example uses a simple console application to prove the exact sequence of events by logging messages directly to the terminal as each phase executes.

```csharp
using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("--- Starting Instantiation ---");
        ChildAccount account = new ChildAccount();
        Console.WriteLine("--- Instantiation Complete ---");
    }
}

public class ParentAccount
{
    // 2. Parent field initializers run second
    private string parentField = LogField("Parent Field Initialized");

    public ParentAccount()
    {
        // 3. Parent constructor runs third
        Console.WriteLine("Parent Constructor Body Executing");
    }

    public static string LogField(string message)
    {
        Console.WriteLine(message);
        return message;
    }
}

public class ChildAccount : ParentAccount
{
    // 1. Child field initializers run first
    private string childField = LogField("Child Field Initialized");

    public ChildAccount()
    {
        // 4. Child constructor runs last
        Console.WriteLine("Child Constructor Body Executing");
    }
}
```

### Expected Output Trace:
```text
--- Starting Instantiation ---
Child Field Initialized
Parent Field Initialized
Parent Constructor Body Executing
Child Constructor Body Executing
--- Instantiation Complete ---
```

---

## Example 2: The Classic Interview Trap (Virtual Methods in Constructors)

Senior developers and technical interview panels frequently use this pattern to test a candidate's deep understanding of the object lifecycle. 

### The Trap Code
Calling a `virtual` method inside a parent constructor is a dangerous anti-pattern in C#. Because of the instantiation order, the overridden method in the child class will execute **before the child constructor has had a chance to initialize its own dependencies or fields**.

```csharp
using System;

public class Program
{
    public static void Main()
    {
        // This will throw a NullReferenceException!
        ChildPrinter printer = new ChildPrinter(); 
    }
}

public class BasePrinter
{
    public BasePrinter()
    {
        Console.WriteLine("BasePrinter Constructor Started.");
        
        // TRAP: Calling a virtual method inside the constructor
        InitPrinter(); 
    }

    public virtual void InitPrinter()
    {
        Console.WriteLine("Base initialization.");
    }
}

public class ChildPrinter : BasePrinter
{
    private List<string> _printQueue;

    public ChildPrinter()
    {
        // The child constructor body runs LAST
        _printQueue = new List<string>(); 
        Console.WriteLine("ChildPrinter Constructor Complete. Queue allocated.");
    }

    public override void InitPrinter()
    {
        Console.WriteLine("Child overriding initialization...");
        
        // CRASH: _printQueue is still null here!
        // Because BasePrinter's constructor invoked this method before 
        // ChildPrinter's constructor body could ever assign the new List().
        _printQueue.Add("Initialization Log"); 
    }
}
```

### Critical Takeaway
Never invoke `virtual` or `abstract` methods from inside a constructor. The runtime will correctly route the call to the overridden method in the derived child class, but because the child's constructor body hasn't executed yet, any local dependencies or collections defined inside that child class will be uninitialized (`null`), resulting in an application crash.
