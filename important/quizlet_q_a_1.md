1. difference between Compile Time and runtime?

The analogy of building a corporate skyscraper:

Compile-Time (Architectural Blueprint stage):
TypeScript: Like drawing lines on paper with a pencil. You can draw anything, but the pencil guidelines are completely erased (type erasure) right before the plans go out. A Compile-Time error here is simply the architect flagging an invalid pencil sketch on the paper before handing it over.
C#: Like creating a strict, 3D digital engineering model. The software checks physical loads and structural measurements. If a line does not connect perfectly, the system throws a Compile-Time error and physically blocks you from printing—the factory machinery refuses to generate the block molds entirely and no assembly (.dll) is produced.

Runtime (Active Construction Site):
TypeScript: The workers build using a generic text description because the guidelines were wiped. If a delivery contains wood instead of steel, they don't realize it until they bolt it together, causing a structural collapse (runtime crash).
C#: The workers are equipped with electronic scanners verifying the real model in real-time (reified types). If someone delivers wood instead of steel, the scanner sounds an immediate alarm (Runtime Exception) before it can be built into the structure.


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

The clipboard on the desk using strict Last-In, First-Out (LIFO). Everytime a method is called, a new sheet(stack frame) is stacked on top of clipboard. Sheet stores method Value Types (int, bool, double, struct, readonly record struct) and reference pointers to larger types stored on the warehouse floor (Heap memory) needed for that method/task. When task completes, sheet is thrown into recycle bin requiring zero help from GC.

*(Note: Value types are only stack-allocated if they are local method variables. If an int lives inside a class container, it is dragged onto the Heap to maintain contiguous memory boundaries).*


4. What is heap memory?

The larger shared warehouse floor. Everytime a large reference object is instantiated using new keyword(heavy class or a standard record), its stored on warehouse floor. The index card(reference pointer) gets clamped to clipboard(stack). When task completes, sheet is thrown into recycle bin, the reference pointer is gone but large object is still on warehouse floor requiring GC to get rid of it.

Other types stored on heap:

Collections - Strings, int[], List<T>, Dictionary<K,V>
Lambda closures - asynchronous tasks, and delegates.
Boxed Value Types - any struct cast to an interface or object

5. Garbage collection (GC):



6. Access modifiers?

To explain visibility parameters seamlessly to a panel using a domestic setting, compare your compiled **Assembly** to a **Physical House Lot**, and your **Classes** to **Family Members**:

*   **`public` (The Front Gate / Street):** Completely open to the public. Anyone walking past your gate, a neighbour, or an external visitor can access it.
*   **`private` (Personal Diary):** Strictly locked. Only you can read your own diary. Your siblings, parents, and outside visitors cannot touch it.
*   **`internal` (Lounge):** Accessible to anyone who lives inside your specific house lot (your **siblings and parents** within the same compiled Assembly). Outside visitors standing on the street cannot access it.
*   **`protected` (The Family Safe):** Only bloodline children (subclasses) can open it. They can access it from home or from a separate house down the street (cross-assembly). An unrelated neighbour class cannot touch it.
*   **`protected internal` (The Driveway):** Open to anyone inside your house lot OR children down the street. Because it is a loose **OR** rule, any unrelated neighbour (separate class with no inheritance) inside your assembly can walk up the driveway and overwrite the asset.
*   **`private protected` (The Kid’s Bedroom):** Accessible *only* to children (subclasses) **AND** they must still physically live inside your exact house lot (same assembly).
*   **`static` (The House Address Sign):** A single blueprint asset fixed permanently to the physical house itself rather than belonging to an individual person. You don't ask an individual sibling what the house number is; you read it directly off the structure.