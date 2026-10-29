1. difference between Compile Time and runtime?

The analogy of building a corporate skyscraper:

Compile-Time (Architectural Blueprint stage):
TypeScript: Like drawing lines on paper with a pencil. You can draw anything, but the pencil guidelines are completely erased (type erasure) right before the plans go out. A Compile-Time error here is simply the architect flagging an invalid pencil sketch on the paper before handing it over.
C#: Like creating a strict, 3D digital engineering model. The software checks physical loads and structural measurements. If a line does not connect perfectly, the system throws a Compile-Time error and physically blocks you from printing—the factory machinery refuses to generate the block molds entirely and no assembly (.dll) is produced.

Runtime (Active Construction Site):
TypeScript: The workers build using a generic text description because the guidelines were wiped. If a delivery contains wood instead of steel, they don't realize it until they bolt it together, causing a structural collapse (runtime crash).
C#: The workers are equipped with electronic scanners verifying the real model in real-time (reified types). If someone delivers wood instead of steel, the scanner sounds an immediate alarm (Runtime Exception) before it can be built into the structure.

1. a. boot up sequence in c#?

# .NET Runtime Boot-Up Sequence

## 1. Turning on the Power Grid (`dotnet run`)
* The Operating System creates a brand new process for your app.
* **The Global Warehouse Floor (Heap):** Instantly mapped out in memory.
* **Static Assets:** Static classes (like `Program`) are built directly onto the warehouse floor, ready for use.

## 2. The Head Chef Arrives (The Main Thread)
* The .NET runtime automatically spawns **one primary worker** called the Main Thread.
* This Head Chef walks directly to the `Program.cs` master switchboard.
* They open up a private **Desk Clipboard (Stack)** to track local variables for the starting method.

## 3. Assembling the Crew (The Thread Pool)
* The runtime opens a backroom filled with idle, pre-hired workers called the **Thread Pool**.
* Instead of creating new workers from scratch (which wastes time), the app keeps these workers resting until needed.

## 4. Allocating the Tasks
* **Synchronous Sequences:** The Head Chef prepares multiple dishes strictly one after the other. If the cake needs to bake for 45 minutes, they stand there staring at the oven door, completely freezing the kitchen until it is 100% complete.
* **Parallel Work:** The manager hires multiple chefs to stand in the kitchen simultaneously. Chef A chops vegetables while Chef B cooks meat on the exact same stovetop at the exact same microsecond.
* **Asynchronous Tasks (`async/await`):** The chef puts a cake in the oven, sets a digital kitchen timer (`await`), and immediately turns around to chop vegetables. When the timer dings, whichever chef is currently free steps over to pull the cake out.


2. What are the foundational architectural pipeline stages of typescript and c#?

*   **TypeScript (3-Step Source-Level Translation Pipeline) — Executed when running `node ...`:**
    > **The Analogy:** A language translator rewriting a document from a foreign script (TS) into local text (JS) so a worker can read it.
    1. **TypeScript Source Code:** High-level human-readable syntax containing static types.
    2. **Transpiled JavaScript:** The static type rules vanish entirely (**type erasure**), leaving raw text.
    3. **Browser Engine / Node.js Runtime:** The event loop host interprets and executes the raw JavaScript text completely blind.

*   **C# / .NET (4-Step Native Hardware Compilation Pipeline) — Executed when running `dotnet run`:**
    > **The Analogy:** An industrial factory system taking a raw design, stamping generic molds, and instantly pouring hard structures onto the ground.
    1. **C# Source Code (`dotnet build` wrapper):** Strong, static object definitions checked rigorously by the Roslyn compiler.
    2. **Intermediate Language (IL Assembly):** Statically compiled generic block molds and type metadata inside a `.dll` or `.exe`.
    3. **JIT Machine Code Compilation (CLR Load):** The runtime host converts generic IL functions into hardware-specific binary on-the-fly upon their first call.
    4. **CPU Register Execution (Live Runtime):** The physical host processor runs native binary instructions guarded by live type scanners.


3. What is stack memory?

The private clipboard on the desk belonging to a single thread, using strict Last-In, First-Out (LIFO). Everytime a method is called, a new sheet(stack frame) is stacked on top of clipboard. Sheet stores method Value Types (int, bool, double, struct, readonly record struct) and reference pointers to larger types stored on the warehouse floor (Heap memory) needed for that method/task. When task completes, sheet is thrown into recycle bin requiring zero help from GC.

*(Note: Value types are only stack-allocated if they are local method variables. If an int lives inside a class container, it is dragged onto the Heap to maintain contiguous memory boundaries).*


4. What is heap memory?

The larger shared warehouse floor Everytime a large reference object is instantiated using new keyword(heavy class or a standard record), its stored on warehouse floor. Every thread walks past it. The index card(reference pointer) gets clamped to clipboard(stack). When task completes, sheet is thrown into recycle bin, the reference pointer is gone but large object is still on warehouse floor requiring GC to get rid of it.

Other types stored on heap:

Collections - Strings, int[], List<T>, Dictionary<K,V>
Lambda closures - asynchronous tasks, and delegates.
Boxed Value Types - any struct cast to an interface or object

5. Garbage collection (GC):



6. Access modifiers?

*   **`public` (The Front Gate / Street):** Completely open to the public. Anyone walking past your gate, a neighbour, or an external visitor can access it.
    *   *Gotcha:* **The Inconsistent Parameter Trap.** You cannot put a method out here on the Front Gate that requires an asset locked in the Lounge (`internal`) as a parameter or return type. The street visitor cannot see or hold an invisible internal type (`Error CS0051`).
    *   *Gotcha:* **The Lineage Constraint.** A child class standing on the Front Gate cannot inherit from a parent class locked inside the Lounge (`internal`). A child cannot be more visible to the public street than their own family lineage (`Error CS0060`).

*   **`private` (Personal Diary):** Strictly locked. Only you can read your own diary. Your siblings, parents, and outside visitors cannot touch it.

*   **`internal` (Lounge):** Accessible to anyone who lives inside your specific house lot (your **siblings and parents** within the same compiled Assembly). Outside visitors standing on the street cannot access it.
    *   *Gotcha:* **The Constructor Illusion.** Marking a constructor as `public` inside a class locked in the Lounge is a silent illusion. External neighbors still cannot see the class wrapper to call that constructor; the outer Lounge perimeter remains locked.
    *   *Gotcha:* **Explicit Intercom Hiding.** An internal family member can prefix a method with its interface name (e.g., `void IIntercom.Ring`) to satisfy a public gate contract while stripping its public keyword. The routine becomes completely invisible on the family member's direct profile unless viewed directly through the gate intercom lens.

*   **`protected` (The Family Safe):** Only bloodline children (subclasses) can open it. They can access it from home or from a separate house down the street (cross-assembly). An unrelated neighbour class cannot touch it.

*   **`protected internal` (The Driveway):** Open to anyone inside your house lot OR children down the street. Because it is a loose **OR** rule, any unrelated neighbour (separate class with no inheritance) inside your assembly can walk up the driveway and overwrite the asset.
    *   *Gotcha:* **The Driveway Leak.** Developers often mistake this for a strict "AND" rule. Even if your architectural intent is family-only access, any completely unrelated neighbor class inside your same house lot can step onto your driveway and alter the data.

*   **`private protected` (The Kid’s Bedroom):** Accessible *only* to children (subclasses) **AND** they must still physically live inside your exact house lot (same assembly).

*   **`static` (The House Address Sign):** A single blueprint asset fixed permanently to the physical house itself rather than belonging to an individual person. You don't ask an individual sibling what the house number is; you read it directly off the structure.





7. Explain Dependency Injection (DI)?

Dependency injection is when passing external dependencies into a class constructor, rather than letting the class instantiate it internally

Why use it?

Satifies DIP by decoupling business logic from concrete implementations

It allows us to swap dependencies for mock objects during unit testing

It hands off object lifetime-management (T, S, S) to the DI container which prevents resource leaks on managed heap.

7.1. What is IOC/DI container ?

IoC stands for Inversion of Control. It is the core architectural principle behind dependency injection, where control over object creation and lifecycles is inverted—handed over to a centralized engine like your Kitchen Cupboard Power Board (Program.cs) instead of individual classes creating their own resources.


7.2. What are the 2 threats of thread-safety on the warehouse floor/heap?

Synchronous concurrency (microsecond collision): Threads/workers going to the same object/pallet at the exact same microsecond tring to mutate the data, bumping into each other. Crashing - causing a runtime error.

Asynchronous interleaving (shift-swap mixup): Worker A walks up to a pallet, reads the inventory, and hits an await (pauses to wait for a forklift). Worker A steps out of the warehouse. While they are gone, Worker B alters the pallet. Worker A returns to finish their task, totally unaware the inventory changed behind their back.


7.3. What are the 5 thread-safety tools?

private readonly _variable - only protects variable name from being re-allocated to another variable in another method. It only protects the object/pallets location reference, other threads/workers can still corrupt data with .Add() because the object is still mutable.

Concurrent collections - Best for shared queue's, lists or dictionaries. Replaces standard lists with smart variations (like ConcurrentDictionary) engineered to handle mutiple/threads/workers without crashing.

Semaphoreslim (Pager system) - Best for async methods that need to protect shared warehouse data. Uses WaitAsync() and Release(). If the object/pallet is busy, threads/workers wait outside the warehouse entirely, freeing up the CPU thread to do other work until the pager buzzes.

Interlocked - Best for incrementing numbers or switching single objects at blistering speeds. Performs operations safely at hardware level without setting up software blocks.

Lock (velvet rope) - Best for sync threads/workers updating shared data on warehouse floor. Adds a rope and lock around object/pallet. Only one thread/worker can step in at a time
**The Rule:** Never use await inside a standard lock (a thread/worker cannot leave the warehouse to wait for a forklift while holding the velvet rope key).



8. Explain 2 types of polymorphism?

Compile-time polymorphism is method-overloading - using same methods but different parameters

Runtime polymorphism is method-overriding - using abstract and virtual keywords in parent classes - the .NET runtime evaluates the type of the physical object in memory and uses the **Virtual Method Table (VMT)** to dynamically dispatch the call.
**The `override` keyword updates the VMT for the object slot**


9. How does exception handling differ when an unhandled failure occurs inside an async Task method compared to an async void method?

async Task - The runtime catches the exception and securely wraps the exception inside the returned Task object.

The exception is only thrown and observed when:

The code executes an await on that specific Task
Inspecting the Tasks .Exception property


async void - The exception escapes the Task pipeline completely and thrown into SynchronizationContext. The caller cannot await an async void method which results in an immediate app crash.

async void should only be utilized for top-level UI event handlers (like button clicks) where async return constraints are structurally mandated.