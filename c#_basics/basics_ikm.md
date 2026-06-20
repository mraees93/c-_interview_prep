1. Language Rules & Core Architecture

1.1. What is the guaranteed behavior of a finally block in a C# try-catch-finally structure when an unhandled exception is thrown inside the catch block?

The finally block will execute completely before the exception propagates up the call stack to crash the application or be caught by a higher handler.

**The finally block is guaranteed to execute regardless of whether the code succeeds or throws an error.



1.2. If a class member is marked as protected internal, which code has access to view or modify that specific member?

Any code within the same project assembly, AND any class in a completely different project assembly as long as it inherits from this class.

**protected internal is often misread as an "AND" condition, but in C# it actually acts as an "OR" condition. It merges two distinct access rules together:
internal: Grant access to any code as long as it sits inside the same project assembly (.dll).
protected: Grant access to any class that inherits from this base class, even if that child class sits in a completely different project assembly.


1.3. for true runtime polymorphism in C#, you must use a pairing mechanism like virtual with override or an abstract method (no body) with override


1.4. Which of the following statements accurately describes how the .NET Common Language Runtime (CLR) Garbage Collector (GC) manages Generation 0?

C) It holds short-lived, newly created objects and is cleared frequently during quick, high-performance optimization sweeps.


1.5. A delegate in C# is a type that safely holds a reference to a method. You can think of it as a type-safe function pointer or a "blueprint" for a method.
It allows you to pass methods as arguments to other methods, store them in variables, and invoke them dynamically at runtime.


1.6. What happens behind the scenes during a boxing operation in C#?

int number = 42;
object obj = number; 

B) A copy of the value type is wrapped and moved from the stack allocation over onto the managed heap allocation as an object reference wrapper.

The Mechanics: In C#, primitives like int, double, and bool live on the lightning-fast execution stack. An object, however, is a reference type that must live on the managed heap.
The Operation: When you assign an int to an object, the runtime has to bridge that gap. It creates a brand-new object shell on the heap, copies the value 42 inside it, and points the variable obj to that heap address.


1.7.When is a static constructor of a class guaranteed to execute within a running application framework instance?

Exactly once, automatically before the first instance of the class is created or any static members are referenced.

You can never call a static constructor manually. It does not have an access modifier (like public or private), takes no parameters, and cannot be invoked by your code.
Instead, the .NET Common Language Runtime (CLR) manages it completely. The runtime waits lazily until the exact moment your code interacts with that class for the very first time.