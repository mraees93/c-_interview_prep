# 🏛️ Domain Architecture: True Encapsulation & Production Traps
*LexisNexis Architectural Panel Defense Module*

Encapsulation is not merely data hiding via `private` fields and public properties—that is a basic textbook definition. True **Encapsulation** is an architectural boundary framework designed to **safeguard domain business invariants and isolate implementation complexity.** It guarantees that an object completely owns its internal state and physically blocks external code from corrupting it.

---

## 🎨 The Kitchen Analogy: The Automated Recipe Vault

* **Fake Encapsulation (The Open Counter Trap):** You make your inventory variable private: `private int _meatWeight;`, but you can still mutate it blindly if you expose a raw public setter: `public void SetMeatWeight(int weight) { _meatWeight = weight; }`. A rogue waiter can execute `SetMeatWeight(-50);`. Your data was technically hidden, but your business rules are completely corrupted because a kitchen cannot have negative fifty kilograms of meat.
* **True Encapsulation (The Protected Vault):** You keep the field private, and your public methods enforce strict, unbreakable kitchen safety guardrails:
  ```csharp
    
    public class CaseInventory
    {
        private int _meatWeight;

        public CaseInventory(int initialWeight)
        {
            _meatWeight = initialWeight;
        }

        public void CookSteak(int requiredWeight)
        {
            if(requiredWeight <= 0) throw new ArgumentException("Cook weight must be completely positive.");

            if(_meatWeight - requiredWeight < 0) throw new InvalidOperationException("Insufficient raw inventory left to execute recipe.");

            _meatWeight -= requiredWeight;
        }
    }

  ```

---

## 🚨 The Production Trap Matrix

| Architectural Tier | 🚨 The Production Trap (The Failure) | ⚙️ Technical Impact on Memory / Execution | 🛡️ The Architecture Fix |
| :--- | :--- | :--- | :--- |
| **Class Properties** | **The Anemic Domain Model Trap:** Exposing automated public getters and setters (`public string Status { get; set; }`) on core entities. | External code bypasses logic gates, mutating data state variables blindly across the heap, leading to data corruption and distributed bugs. | Convert properties into `private set` or `init` properties. Force all state transformations to pass through explicit, validated domain methods. |
| **Collection Leaks** | **The Exposed Reference Leak:** Returning a raw backing list from an entity property (e.g., `public List<Document> Docs => _docs;`). | **Reference Type Copying:** The external caller receives a pointer straight to the private heap list, allowing them to call `.Clear()` or `.Add()` out-of-band. | Expose collections as `IReadOnlyCollection<T>` or `IEnumerable<T>`. Materialize internal modifications via explicit class backing hooks. |
| **Assembly Scopes** | **The Public-By-Default Leak:** Marking every repository and utility helper class as `public` across your infrastructure layer. | Completely breaks encapsulation at the DLL boundary, permitting API controller projects to bypass interfaces and instantiate database dependencies directly. | Enforce the `internal` access modifier on all concrete data access implementations. Only expose the clean structural Interface contracts to external assemblies. |

---

## 🧱 3. Assembly Scope Encapsulation: The Internal Hard Ceiling

* **The Pattern:** A structural combination of the **Dependency Inversion Principle (SOLID)** and the **Composition Root Pattern**.
* **The Core Rule:** Mark concrete infrastructure or data classes as **`internal`** instead of `public` inside your data access project assembly (.csproj) [ON, SUN, JUNE 21, 2026 @ 16:32 PM]. Expose *only* a `public` contract Interface [ON, SUN, JUNE 21, 2026 @ 16:32 PM].
* **The Method Hard Ceiling:** A member's visibility can never exceed its parent class. Making a class `internal` establishes an unbreakable visibility box—all inner methods automatically become internal to outside projects, even if explicitly written as `public`.
* **The Production Win:** The external API controller is physically blocked from discovering or instantiating the concrete database plumbing class directly on the heap. This prevents tight architectural coupling and forces the API layer to rely solely on clean abstractions.

---

### 📜 Architectural Contract: ICaseRepository.cs
*Marked public so the external Web API assembly can see the contract handles.*

```csharp
namespace LexisNexisWorkspace.Modules.Cases.Domain;

public interface ICaseRepository
{
    Task<IEnumerable<string>> GetActiveCaseTitlesAsync();
}
```

---

### 🛋️ Infrastructure Implementation: SqlCaseRepository.cs
*Marked internal to lock it inside the private assembly lounge.*

```csharp
using LexisNexisWorkspace.Modules.Cases.Domain;

namespace LexisNexisWorkspace.Modules.Cases.Infrastructure;

// 🔒 THE VAULT: External assemblies cannot see or compile against this type!
internal class SqlCaseRepository : ICaseRepository
{
    // 🧱 THE HARD CEILING: Trapped inside an internal class, this method is implicitly internal.
    // Unrelated external assemblies can NEVER call or see this method directly.
    public async Task<IEnumerable<string>> GetActiveCaseTitlesAsync()
    {
        return await Task.FromResult(new List<string> { "S v Zuma", "State v Maharaj" });
    }
}
```

---

### 🏢 External Web API Consumer: CasesController.cs
*Lives in a completely separate project assembly across the street.*

```csharp
using Microsoft.AspNetCore.Mvc;
using LexisNexisWorkspace.Modules.Cases.Domain; // 👈 Only allowed to import the public contract

namespace LexisNexisWorkspace.Modules.Cases.Api;

[ApiController]
[Route("api/cases")]
public class CasesController : ControllerBase
{
    private readonly ICaseRepository _repository; // ✅ LEGAL: Binds to the public interface contract

    public CasesController(ICaseRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary()
    {
        // 💥 THE INTRUSION FAILURE TRAIL:
        // var localRepo = new SqlCaseRepository(); // ❌ THROWS COMPILER BUILD ERROR!
        
        var data = await _repository.GetActiveCaseTitlesAsync(); // ✅ LEGAL: Executes via abstract gate
        return Ok(data);
    }
}
```

### ❓ Panel Defense: "If it's internal, how does the container inject it?"
> "We use the **Composition Root Pattern**. The Data assembly exposes a public service registration extension method. Because that initialization method lives **inside the same assembly house** as the internal class, it has full clearance to instantiate `SqlCaseRepository` on the heap and bind it to the public `ICaseRepository` service collection. The external API layer never discovers the underlying class metadata."

---

## 📋 High-Frequency Panel Interview Questions

### ❓ Q1: Is encapsulation achieved solely by making fields private and exposing them via public properties?
* **The Punchy Answer:** "No. That is just data hiding with extra steps. True encapsulation is about **protecting business invariants and hiding implementation complexity**."
* **The Panel Defense:** "If you expose a private field through a blind public property setter, external code can still force your object into an invalid state. True encapsulation ensures a class entirely dictates its internal state, acting as a strict validation gate that rejects any mutations violating domain rules."

### ❓ Q2: If you make a class `internal` but mark a method inside it as `public`, can an external project see that method?
* **The Punchy Answer:** "No. A member's visibility can never exceed the accessibility boundary of its parent class."
* **The Panel Defense:** "When a class is marked `internal`, its entire type metadata is sealed inside that compiling assembly (DLL). Even if a method inside it uses the `public` modifier keyword, the parent class acts as a hard ceiling. An external assembly can never resolve or instantiate the class type, blocking access to all inner methods automatically."

### ❓ Q3: Why is `private protected` used almost exclusively on class methods rather than the classes themselves?
* **The Punchy Answer:** "Because top-level classes cannot be marked `private protected` by the compiler; it is explicitly designed as an internal inheritance lock for class members."
* **The Panel Defense:** "The purpose of `private protected` is to restrict access to derived subclasses within the same project assembly. Since top-level classes do not have an inheritance parent context above them at the file system root, the modifier only makes sense when applied directly to **Methods, Fields, or Nested Classes** inside a class hierarchy."
