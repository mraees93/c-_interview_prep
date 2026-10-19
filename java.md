# C# to Java Reference Guide for Enterprise Interviews

## 🏛️ 1. Core Architectural Ecosystem Mapping
When discussing the master application switchboard and database pipelines, map your .NET terms directly to the Java equivalents used at LexisNexis:

| C# / .NET Core Term | Java / Spring Boot Equivalent | The Universal Kitchen Analogy |
| :--- | :--- | :--- |
| **Assembly / Project** | **JAR / WAR / Module** | The physical **House Lot** container your code compiles into. |
| **Namespace** | **Package** | The internal directory folder layout separating your classes. |
| **`Program.cs` / DI Container** | **Spring ApplicationContext** | The master kitchen breaker board managing lifetimes. |
| **Entity Framework Core (EF Core)** | **Hibernate / Spring Data JPA** | The database object mapper pulling data boxes out of trucks. |
| **API Controllers (`[HttpGet]`)** | **`@RestController` (`@GetMapping`)** | The public counter receiving incoming customer tickets. |

---

## 🔒 2. Visibility Nuances (The Lounge vs. The Room)
* **In C# (`internal`):** Visibility is locked to the entire compiled assembly (the whole House Lot). Any family member anywhere inside that House Lot can step into the Lounge.
* **In Java (`default` / package-private):** If you omit an access modifier keyword next to a class, Java defaults visibility to the **Package** (a specific room layout). Only family members standing inside that *exact same folder directory* can see it. If a family member lives in a sibling folder in the same project, they are completely locked out.

---

## 🛠️ 3. Basic Code Syntax Translations

### A. Console & Base Primitives
```csharp
// C#
Console.WriteLine("Data");
string title = "Zuma Case";
bool isActive = true;
```
```java
// Java
System.out.println("Data");
String title = "Zuma Case"; // Capital 'S' for String!
boolean isActive = true;    // Lowercase 'b' for primitive boolean!
```

### B. Standard Lists & Collections
Java collections live completely on the **Warehouse Floor (the Heap)** as objects. Java does not support custom `struct` value types.
```csharp
// C#
List<string> cases = new() { "Case 1" };
cases.Add("Case 2");
int total = cases.Count;
```
```java
// Java
List<String> cases = new ArrayList<>();
cases.add("Case 1");
cases.add("Case 2");
int total = cases.size(); // Uses .size() instead of .Count!
```

### C. The Data Pipeline (LINQ vs. Streams)
In Java, collections cannot be filtered directly. You must explicitly open a conveyor belt (`.stream()`), process the data via lambda callback functions (`->`), and collect it back into a list.
```csharp
// C# LINQ
var active = cases.Where(c => c.IsActive).ToList();
```
```java
// Java Streams API (Java 8+)
List<LegalCase> active = cases.stream()
    .filter(c -> c.isActive())
    .collect(Collectors.toList());
```

---

## ⚠️ 4. Crucial Structural "Gotchas"

### Gotcha 1: Properties vs. Explicit Methods
Java does not have native `{ get; set; }` auto-properties. You declare a strict private field and explicitly call methods.
* *C# Style:* `myCase.Price = 100;`
* *Java Style:* `myCase.setPrice(100);`
* *Enterprise Fix:* Real enterprise projects use a library called **Lombok**. You put `@Getter` and `@Setter` above your Java class, and it automatically injects those method pipelines at compile time behind the scenes.

### Gotcha 2: Checked Exceptions (Java's Strict Rule)
* **In C#:** All exceptions are unchecked. The compiler never forces you to wrap code in a `try/catch`.
* **In Java:** If a method reads from a file, database, or external network driver, the compiler **forces** you to either handle it inside a `try/catch` block or declare `throws IOException` on your method signature, otherwise it will refuse to build.
