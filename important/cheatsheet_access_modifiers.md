# 🛡️ Access Modifiers & Assembly Boundaries - Core Panel Cheatsheet
*LexisNexis Interview Preparation - Architectural Access Control Module*

---

## 🎭 The Real-World Analogy: The Corporate Legal Headquarters

To explain visibility parameters seamlessly to a panel, compare it to a high-security corporate building:

*   **Public:** The public lobby on the ground floor. Anyone walking down the street can walk inside.
*   **Private:** A single lawyer's locked desk drawer. Only that exact lawyer possesses the key; nobody else in the entire firm can look inside.
*   **Internal:** The employee-only office floor. Anyone with a corporate staff badge (**the compiled Assembly**) can access it, but the general public is strictly locked out.
*   **Protected:** A family trust fund legacy asset. Access is granted strictly via inheritance down the bloodline (**Subclasses**), regardless of where the heirs are currently located.

---

## ⚙️ The Architectural Modifiers Matrix

C# features 6 explicit access levels. The two complex combinations are favorite target filters for technical panels:

| Modifier Name | Visual Boundary Definition | Cross-Assembly Access Parameter |
| :--- | :--- | :--- |
| **`public`** | Completely unrestricted visibility. | Fully visible to any external assembly referencing this project. |
| **`internal`** | Visible *only* within the same compiled assembly. | Statically invisible to external code packages. |
| **`protected`** | Visible within the same class **and** any derived subclasses. | Allowed cross-assembly *exclusively* down the inheritance line. |
| **`private`** | Strictly visible *only* within the declaring class block. | Completely hidden from outside view. |
| **`protected internal`** | **OR Boundary:** Same Assembly **OR** Subclasses anywhere. | Accessible if a cross-assembly class explicitly inherits from it. |
| **`private protected`** | **AND Boundary:** Subclass **AND** Same Assembly *only*. | Statically blocked from cross-assembly access, even with inheritance. |

---

## 🛡️ Immediate Panel Defense Scripts

### Trap A: The Leaky Cross-Assembly Property (`protected internal`)
*   **The Disaster:** A developer uses `protected internal` to expose a data attribute (like a `CaseId`), assuming it can only be modified by derived subclasses. However, because it opens an **"OR"** boundary, any completely unrelated, random class inside that exact same assembly can now access and overwrite that value, shattering encapsulation.
*   **The Defense:** *"I implement `private protected` if I want to restrict access strictly to our specific subclass inheritance tree, guaranteeing that it remains completely locked out from non-derived classes inside our same assembly, and fully invisible to external assemblies."*

### Trap B: Breaking the Component Scoping Limit
*   **The Disaster:** Declaring an interface implementation or a base class as `public` when it is only used internally by background processing engines. This exposes inner system components to external consumers, creating fragile compile-time dependencies that break whenever the core code changes.
*   **The Defense:** *"I default all structural components and framework classes to `internal`. By restricting visibility to our compiled assembly boundary, we protect our core engine from external tampering and can refactor code freely without breaking consumer systems."*

---

## 🚨 Live Coding Evaluation Trap: The Visibility Constraint Rule

A favorite trick used by panels to see if you actually understand compiler assembly rules is handing you a class block that introduces an **inconsistent accessibility** compilation failure:

```csharp
// Hidden, non-public database component
internal class LegalDatabaseConnection { }

// The Panel Trap: Public service exposes an internal component
public class CaseAuditor
{
    // ❌ CRITICAL COMPILER ERROR: Inconsistent accessibility!
    public LegalDatabaseConnection GetActiveConnection() 
    {
        return new LegalDatabaseConnection();
    }
}
```

#### The Exact Panel Breakdown:
*   **The Problem:** The method `GetActiveConnection` is marked `public`, meaning external consumers can call it. However, it attempts to return an `internal` type (`LegalDatabaseConnection`) that external consumers cannot physically see or compile against. The compiler catches this impossibility and rejects the build immediately.
*   **The Safe Remediation:** You must match the visibility constraints. If the structural data model returned is strictly internal to the engine, the wrapping orchestration class or method signature must be demoted to `internal` as well.
