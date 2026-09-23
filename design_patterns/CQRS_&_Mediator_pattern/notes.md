# 🔀 System Design Architecture: CQRS & Mediator Patterns
*LexisNexis Advanced Enterprise Operations Module*

When preparing for an enterprise architecture panel like LexisNexis, you must frame **CQRS (Command Query Responsibility Segregation)** and the **Mediator Pattern** as the ultimate strategy to completely decouple your API controllers from your core business logic and database read/write nodes.

---

## 🎨 The Kitchen Analogy: The Head Waiter & The Split Prep Lines

* **The Old Way (Tight Coupling):** The Customer calls the Line Chef directly to ask what specials are left, or to complain about an ingredient. The Chef is swamped, trying to take phone orders, look up spreadsheets, and cook steaks simultaneously. The system breaks under high load.
* **The Mediator Way (The Head Waiter):** The API controller doesn't talk to data tiers. It simply creates an explicit request voucher packet—either a **Command** (an instruction to change something, like an order) or a **Query** (a request to read something, like looking at the menu). The controller drops this packet directly onto the desk of **The Head Waiter (The Mediator)**. 
* **The CQRS Way (Split Kitchen Pipelines):** The Head Waiter reads the voucher packet. 
  * If it's a **Query**, he hands it strictly to the **Cold Salad Salad Bar (The Read Replica Cluster)** which effortlessly returns cached data instantly.
  * If it's a **Command**, he hands it strictly to the **Hot Grilling Station (The Primary Write Node)** which enforces intense validation rules, updates the inventory ledger, and commits transactions. 

The API controller never knows which chef cooked the meal or how the database was structured—it only knows the Head Waiter.

---

## 🏛️ The CQRS & Mediator Component Mapping

To keep your code exceptionally organized and bypass any potential context cut-offs, the pattern implementation is broken down here into **four clean, individual structural files**:

***

### 📜 File 1: The Write Contract (The Command & Handler Package)
*Commands represent task-oriented business intents that mutate data on the heap. They return zero data or a tiny operation token (like a Guid).*

### 🔌 Third-Party Interfaces: MediatR Compilation Type Markers

* **The Dependency Source:** `IRequest<T>` and `IRequestHandler<T, U>` are third-party interface structures imported via the **MediatR NuGet Package**, completely independent of the baseline native .NET Core assemblies.
* **The Marker Interface Mechanics:** `IRequest<T>` implements a pure **Marker Interface Pattern**. It contains zero internal properties or fields. It serves strictly as a generic type marker telling the compilation pipeline what data shape the transaction will return upon completion.
* **The Behavioral Contract:** `IRequestHandler` enforces zero data structures on the heap. It mandates a singular execution gateway method (`Handle`), forcing concrete execution classes to safely unpack request payloads out-of-sight of the API controller.

```csharp
using MediatR;

namespace LexisNexisWorkspace.Modules.Cases.Commands;

// 1. THE COMMAND PACKET RECORD (Immutable Transaction Intent)
public record CreateCaseCommand(string Title, string PracticeArea) : IRequest<Guid>;

// 2. THE HANDLER EXECUTOR (The Dedicated Hot Grilling Station Chef)
internal class CreateCaseCommandHandler : IRequestHandler<CreateCaseCommand, Guid>
{
    private readonly CaseWriteDbContext _writeContext; // Targets Primary Node (Writes)

    public CreateCaseCommandHandler(CaseWriteDbContext writeContext)
    {
        _writeContext = writeContext; // Injected via Composition Root Root
    }

    public async Task<Guid> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
    {
        // Enforce true encapsulation validation checks before mutating state
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Case title cannot be blank.");

        var caseEntity = new CaseEntity { Id = Guid.NewGuid(), Title = request.Title, Status = "Active" };
        
        _writeContext.Cases.Add(caseEntity);
        await _writeContext.SaveChangesAsync(cancellationToken);

        return caseEntity.Id; // Return only the identity tracking token token
    }
}
```

***

### 📜 File 2: The Read Contract (The Query & Handler Package)
*Queries represent lightweight, read-only data requests. They bypass heavy domain entities and map directly to optimized DTO structures.*

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LexisNexisWorkspace.Modules.Cases.Queries;

// 1. THE QUERY PACKET RECORD (Idempotent Filter Intent)
public record GetCaseByIdQuery(Guid CaseId) : IRequest<CaseDocketDto>;

// 2. THE DTO DATA MATERIALIZATION RECORD (POCO View)
public record CaseDocketDto(Guid Id, string Title, string Status);

// 3. THE HANDLER EXECUTOR (The Dedicated Cold Salad Bar Chef)
internal class GetCaseByIdQueryHandler : IRequestHandler<GetCaseByIdQuery, CaseDocketDto>
{
    private readonly CaseReadDbContext _readContext; // Targets Replica Clusters (Reads)

    public GetCaseByIdQueryHandler(CaseReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task<CaseDocketDto> Handle(GetCaseByIdQuery request, CancellationToken cancellationToken)
    {
        // 🚀 EFFICIENCY WIN: Direct projection into a DTO automatically turns off the change tracker!
        var dto = await _readContext.Cases
            .Where(c => c.Id == request.CaseId)
            .Select(c => new CaseDocketDto(c.Id, c.Title, c.Status))
            .FirstOrDefaultAsync(cancellationToken);

        if (dto == null)
            throw new KeyNotFoundException("The requested case does not exist inside storage archives.");

        return dto;
    }
}
```

***

### 📜 File 3: The Decoupled Consumer (The Thin API Controller)
*The Controller handles nothing but HTTP routing. It is 100% agnostic of how business rules or database handlers are compiled.*

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LexisNexisWorkspace.Modules.Cases.Commands;
using LexisNexisWorkspace.Modules.Cases.Queries;

namespace LexisNexisWorkspace.Modules.Cases.Api;

[ApiController]
[Route("api/cases")]
public class CasesController : ControllerBase
{
    private readonly IMediator _mediator; // 🧠 The Head Waiter Interface Handle

    public CasesController(IMediator mediator)
    {
        _mediator = mediator; // Composed solely of the mediator router tool
    }

    // 🥇 READ OPERATION: Routed automatically to the Query Pipeline
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCaseById([FromRoute] Guid id)
    {
        // Drop the query packet onto the mediator counter and await response
        var result = await _mediator.Send(new GetCaseByIdQuery(id));
        return Ok(result); // 200 OK
    }

    // 🥈 WRITE OPERATION: Routed automatically to the Command Pipeline
    [HttpPost]
    public async Task<IActionResult> CreateCase([FromBody] CreateCaseCommand command)
    {
        // Drop the command packet onto the mediator counter
        var newCaseId = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetCaseById), new { id = newCaseId }, new { id = newCaseId }); // 201 Created
    }
}
```

***

## 💾 The CQRS & Mediator Cheat Sheet (MD Format to Copy)

You can append this short, punchy summary straight into your master architecture files folder:

```markdown
## 🔀 Structural Architecture: CQRS & MediatR Execution Physics

Implementing segregated processing pipelines completely isolates application transport protocols from infrastructure data stores.

### 1. The Mediator Pattern (MediatR Ingestion)
* **The Architecture Rule:** Replaces tight constructor injection dependencies with a single messaging counter (`IMediator`). Controllers publish Request objects (Commands/Queries) down an in-memory bus channel. 
* **The Execution Physics:** The mediator maps the request type against its corresponding handler registration table on the heap, executing code paths out-of-sight. This cuts down controller dependencies to exactly **one wrapper interface tool**.

### 2. CQRS: Read/Write Split Contraction
* **The Command Pipeline (Writes):** Focused entirely on domain business behaviors and rule-invariant safety checks. Intercepts mutations, writes transactions to the primary database node, and returns no heavy entities—preserving write-path encapsulation boundaries.
* **The Query Pipeline (Reads):** Designed for raw data-materialization velocity. Bypasses domain validation layers entirely, hits the read-replica data clusters, and maps database tables straight into clean custom DTO objects, automatically stepping down the ORM's memory-heavy tracking configurations.
```

***

## 📋 Top Technical Panel Interview Questions

### ❓ Q1: Why use the Mediator pattern inside your controllers? What problem does it actually solve?
* **The Punchy Answer:** "It completely eliminates **Constructor Bloat** and decouples our routing endpoints from changing business handlers."
* **The Panel Defense:** "Without a mediator, as a controller grows to handle 5 or 6 endpoints, you are forced to inject 5 or 6 separate service dependencies into its constructor window, cluttering its heap profile and creating a brittle, tightly coupled class blueprint. With a mediator like MediatR, the controller requires exactly **one dependency: `IMediator`**. If we append new business logic features or modify internal database handlers down the wire, the controller's code signature remains completely frozen and unaffected."

### ❓ Q2: Does CQRS require you to run two completely separate databases?
* **The Punchy Answer:** "No. You can implement CQRS flawlessly inside a single database instance by separating your **code optimization boundaries**."
* **The Panel Defense:** "While advanced systems use separate physical read and write databases (like an event store paired with Elasticsearch), you achieve immediate enterprise value by separating your software execution tracks. In our C# architecture, we implement CQRS inside one database by splitting our DbContext profiles: `CaseWriteDbContext` maps to the master write-node with full tracked capabilities, while `CaseReadDbContext` connects to read-replicas with strict `.AsNoTracking()` projections, maximizing memory and thread performance without data duplication overhead."
