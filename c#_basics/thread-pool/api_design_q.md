# 🌐 Top Technical Interview Questions: RESTful APIs & Controller Mechanics
*LexisNexis Architectural Panel Defense Module*

---

### ❓ Q1: What is the difference between inheriting from `ControllerBase` vs. `Controller` when building a Web API in .NET Core?

*   **The Answer:** Always inherit from **`ControllerBase`** for pure backend REST APIs. 
*   **The Technical Reason:** The standard `Controller` class inherits from `ControllerBase` but adds extra plumbing to support traditional MVC Views (like Razor pages, `ViewData`, and HTML rendering engines). For a high-performance REST API returning raw JSON, inheriting from `Controller` introduces unnecessary memory footprint and object allocation overhead on the managed heap. `ControllerBase` contains only the core mechanics needed for API routing, authorization, and response contracts (`Ok`, `Created`, `BadRequest`).

---

### ❓ Q2: What are the primary benefits of adding the `[ApiController]` attribute to your controller class layout?

*   **The Answer:** It enforces automated, enterprise-grade safety guardrails, drastically cutting down on redundant boilerplate code.
*   **The 3 Core Behaviors:**
    1.  **Automatic 400 Validation Short-Circuiting:** If an incoming JSON payload violates validation rules (e.g., `[Required]` or `[StringLength]`), the framework instantly intercepts the request thread and returns a `400 Bad Request` *before* the request ever hits your controller action method. This completely protects your Thread Pool from wasting CPU cycles on broken traffic.
    2.  **Inferred Binding Source Rules:** It forces the runtime to guess parameters smartly without forcing you to write repetitive attributes (e.g., complex objects are automatically parsed `[FromBody]`, and basic types like Guids are parsed `[FromRoute]` or `[FromQuery]`).
    3.  **Standardised Error Responses:** It formats invalid model states into a structured RFC-compliant JSON schema error response automatically.

---

### ❓ Q3: What does Idempotency mean in REST design? Which HTTP verbs are idempotent, and why is POST the exception?

*   **The Answer:** An operation is **Idempotent** if making multiple identical network requests yields the exact same system state outcome as making a single request.
*   **The Division:**
    *   **Idempotent Verbs (`GET`, `PUT`, `DELETE`):** Calling `GET /api/cases/5` one hundred times will never mutate database records. Calling `DELETE /api/cases/5` multiple times achieves the same final outcome—the record is gone (even though subsequent calls return a 404, the database *state* remains unchanged).
    *   **Non-Idempotent Verb (`POST`):** A `POST /api/cases` execution commands the server engine to create a completely new object on the infrastructure heap. If a network blip occurs and a client re-submits the exact same `POST` payload 5 times, your database will duplicate the entries and instantiate 5 distinct records with unique IDs, causing major transactional collisions.

---

### ❓ Q4: When designing an API for high-velocity, write-heavy ingestion lines, why would you return a `202 Accepted` status code instead of a `201 Created`?

*   **The Answer:** You return a **`202 Accepted`** to break synchronous processing bottlenecks and offload the transaction out-of-band.
*   **The Mechanical Performance Difference:**
    *   **`201 Created` (Synchronous Dependency Block):** The Web API thread must intercept the payload, block its execution loop while waiting on the database context to complete disk writes (`SaveChangesAsync`), retrieve the new database ID, and *then* return a response. Under heavy traffic, this triggers severe thread pool exhaustion.
    *   **`202 Accepted` (Asynchronous Hand-off):** The Web API thread intercepts the payload, instantly pushes it as a raw byte packet into a **RabbitMQ queue**, and immediately returns a `202 Accepted` response back to the client in under 15 milliseconds. The connection is closed, the user interface remains responsive, and the heavy database write work is performed completely down the wire by background competing consumers at a resource-safe pace.

---

### ❓ Q5: What is the mechanical difference between a Route Parameter and a Query String Parameter, and when do you use each?

*   **The Answer:** Route parameters define the physical address boundary of a resource, while query strings modify or filter how that resource data is presented.
*   **The Structural Use Cases:**
    *   **Route Parameter (`[FromRoute]`, e.g., `/api/cases/{id}`):** Used to identify a *specific, unique resource* by its primary lookup key (e.g., fetching, updating, or deleting Case ID `5`). If the route parameter is omitted, the URL fundamentally changes, pointing to an entirely different room or collection.
    *   **Query String Parameter (`[FromQuery]`, e.g., `/api/cases?status=Active&page=2`):** Used for optional parameters like **sorting, filtering, or pagination columns**. The primary endpoint path remains identical, but the query string acts as a modifier telling the underlying repository loop how to segment the returned dataset.
