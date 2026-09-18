# 🌐 RESTful API Architecture Boundaries & Controller Mechanics

## 🏛️ The Controller Infrastructure Framework

In ASP.NET Core, an API controller serves as the entry gate that intercepts a network HTTP request thread and routes it to your underlying domain services.

1. **`ControllerBase` vs `Controller`:** Always inherit from **`ControllerBase`** when building REST Web APIs. The standard `Controller` class adds unnecessary overhead because it includes built-in support for rendering MVC Views (HTML pages), which creates memory bloat when you only need to return raw JSON data.
2. **`[ApiController]` Attribute:** Appending this macro at the top of your class enforces strict enterprise API behaviors. It automatically returns a `400 Bad Request` if model validation rules fail (short-circuiting before wasting CPU thread cycles), and forces incoming parameters to cleanly bind from request headers or bodies.
3. **`[Route("api/[controller]")]`:** Defines the central routing token line. The token `[controller]` automatically reads your class prefix (e.g., `CasesController` becomes `/api/cases`).
4. **IActionResult** is a C# interface contract that defines what a controller returns back to the web browser after handling an HTTP request. Think of it as a generic wrapper or box. Instead of forcing a method to strictly return only data (like a string or a full legal case class), IActionResult gives the thread the absolute flexibility to return both data AND the correct HTTP status code simultaneously

---

## 💻 Micro-Snippet: The Unified Enterprise Controller Layout

This is a production-grade template implementing clean constructor injection, routed replica read/write boundaries, and explicit REST status code contracts:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LexisNexisWorkspace.Modules.Cases.Api;

[ApiController]
[Route("api/cases")] // 🔌 Central Route Window: /api/cases
public class CasesController : ControllerBase
{
    private readonly ICaseRepository _repository;

    public CasesController(ICaseRepository repository)
    {
        _repository = repository; // Injected via IoC container onto the heap
    }

    // 🥉 1. GET ALL (Idempotent Resource Retrieval)
    // Route: GET /api/cases
    [HttpGet]
    public async Task<IActionResult> GetAllActiveCases()
    {
        var cases = await _repository.GetActiveSummaryListAsync();
        return Ok(cases); // Returns 200 OK with JSON array payload
    }

    // 🥈 2. GET SINGLE (Idempotent Specific Query)
    // Route: GET /api/cases/d3b07384-d113-4953-a558-111111111111
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCaseById([FromRoute] Guid id)
    {
        var caseRecord = await _repository.GetReadOnlyCaseAsync(id);

        if(caseRecord == null) 
        {
            return Not Found($"Case resource with ID {id} was not found.");
        }

        return Ok(caseRecord);
    }

    // 🥇 3. POST (Non-Idempotent Resource Creation)
    // Route: POST /api/cases
    [HttpPost]
    public async Task<IActionResult> CreateNewCase([FromBody] CreateCaseCommand command)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState); // 400 Short-circuit safety net

        var newCaseEntity = await _repository.SaveTransactionalCaseAsync(command);
        
        // Returns 201 Created with a location header pointing back to the unique resource
        return CreatedAtAction(nameof(GetCaseById), new { id = newCaseEntity.Id }, newCaseEntity);
    }

    // 🔀 4. PUT (Idempotent Complete Object Replacement)
    // Route: PUT /api/cases/d3b07384-d113-4953-a558-111111111111
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEntireCase([FromRoute] Guid id, [FromBody] UpdateCaseDto updateData)
    {
        var updated = await _repository.UpdateWholeRecordAsync(id, updateData);
        
        if (!updated)
            return NotFound();

        return NoContent(); // Returns 204 No Content (Standard successful replacement update response)
    }

    // 💀 5. DELETE (Idempotent Destruction Contract)
    // Route: DELETE /api/cases/d3b07384-d113-4953-a558-111111111111
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCaseRecord([FromRoute] Guid id)
    {
        var deleted = await _repository.DeleteRecordPermanentlyAsync(id);
        
        if (!deleted)
            return NotFound();

        return NoContent(); // Returns 204 No Content (Structural payload destruction complete)
    }
}
```

---

# 🌐 RESTful API Architecture Boundaries

REST APIs act as the deterministic front-counter contract that maps incoming network traffic directly into active Thread Pool worker threads.

### 1. HTTP Verb Commitments (The Clear Menu)
* **`GET /cases`** -> Retrieves a collection. Must be completely idempotent (safe to call 100 times without mutating database state).
* **`POST /cases`** -> Submits a brand-new payload to execute writes on the infrastructure heap.
* **`PUT /cases/{id}`** -> Performs a complete object replacement/update at a specific target boundary.
* **`DELETE /cases/{id}`** -> Completely terminates a specific unique data record resource.

### 2. Status Code Contracts (The Kitchen Feedback)
* **`200 OK`** -> The thread successfully completed the calculation and returned the payload data.
* **`201 Created`** -> A `POST` request successfully committed a new record to the database heap.
* **`202 Accepted`** -> Used for high-throughput write streams. The web API thread dropped the payload into a RabbitMQ queue and closed the connection instantly in 15ms, offloading the heavy work to async workers.
* **`400 Bad Request`** -> Client validation failure. The request parameters are broken, short-circuiting execution before wasting CPU threads.
* **`401 Unauthorized`** vs. **`403 Forbidden`** -> 401 proves you don't have a valid signature (Authentication); 403 proves your signature is real, but you don't have permission to touch that room (Authorization).
* **`500 Internal Server Error`** -> A domain exception slipped past your guards, caught centrally by your Global Error Middleware.
