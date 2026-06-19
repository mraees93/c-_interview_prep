# .NET Architectural Reference: Compilation Boundaries & Access Modifiers

This guide breaks down the structural hierarchy of a Microsoft .NET application, detailing how the compiler groups code files and how access keywords interact across these boundaries.

---

## 1. The .NET Structure Hierarchy

In the .NET ecosystem, the compiler does not look at your physical Windows file folders to determine access permissions. Instead, it evaluates code based on a strict logical hierarchy:

  ┌────────────────────────────────────────────────────────┐
  │ 1. THE SOLUTION (.sln)                                 │
  │    (The main container linking all related projects)    │
  │                                                        │
  │    ┌────────────────────────┐  ┌─────────────────────┐ │
  │    │ 2. PROJECT A (.csproj) │  │ 3. PROJECT B        │ │
  │    │    (e.g., Core Web API)│  │    (e.g., Core Logs)│ │
  │    │                        │  │                     │ │
  │    │    [File1.cs]          │  │    [File3.cs]       │ │
  │    │    [File2.cs]          │  │                     │ │
  │    └───────────┬────────────┘  └───────────┬─────────┘ │
  └────────────────┼───────────────────────────┼───────────┘
                   ▼                           ▼
       Compiles into an ASSEMBLY   Compiles into an ASSEMBLY
       (e.g., WebApi.dll)          (e.g., CoreLogs.dll)

### Component Breakdown

*   **The Code Files (`.cs`):** The individual text files where you write your C# classes, structs, and interfaces.
*   **The Project (`.csproj`):** A logical configuration file that groups related code files together to complete a unified task (e.g., managing database access or processing calculations). Each project sits inside its own sub-folder.
*   **The Assembly (`.dll` or `.exe`):** When you compile a project, the .NET compiler packages all the code files within that project into a single binary file. One Project = One Assembly.
*   **The Solution (`.sln`):** The highest master container. It maps and links multiple related projects together so they can be developed and compiled simultaneously inside your IDE.

---

## 2. Decoupling Access Modifiers: protected internal

Understanding assembly boundaries is essential for decoding complex C# access level flags on corporate tests like IKM. The keyword combination `protected internal` acts as a logical "OR" operation rather than an "AND" condition.

### The Two Component Rules
1.  **`internal` (Assembly Boundary):** Grants visibility to any code file that compiles into the same specific Assembly (`.dll`), regardless of whether the classes are related.
2.  **`protected` (Inheritance Boundary):** Grants visibility to any class that inherits from this base class, even if that child class lives in a completely different project, different folder, and compiles into a completely different Assembly (`.dll`).

### Summary Table

| Access Modifier | Visible Within Same Project Assembly? | Visible in Different Project Assembly? |
| :--- | :--- | :--- |
| **`internal`** | Yes (Any class) | No |
| **`protected`** | Yes (Subclasses only) | Yes (Subclasses only) |
| **`protected internal`** | **Yes (Any class)** | **Yes (Subclasses only)** |

*Note on Modern C# Versions:* If a design requires a strict "AND" boundary—where a member must live inside the same project assembly AND inherit from the class—you must utilize the **`private protected`** modifier.
