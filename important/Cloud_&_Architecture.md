# ☁️ Full-Stack Cloud & Architecture Recall Module
*LexisNexis Interview Preparation - Shortened Quick-Review Sheet*

---

## ⚙️ 1. Core .NET Framework Mechanics

### Dependency Injection (DI) & Lifecycles
*   **Concept:** DI is a native IoC (Inversion of Control) container that manages object instantiation centrally, decoupling classes from concrete implementations to allow seamless testing and mocking.
*   **Lifecycles:**
    *   **Transient:** A brand-new instance is created *every single time* it is requested. Best for lightweight, stateless utilities.
    *   **Scoped:** Exactly *one instance* is created per HTTP request lifecycle. Shared across that web call and disposed of at completion. *Standard for the EF Core `DbContext`.*
    *   **Singleton:** Exactly *one instance* is created at application startup and lives for the entire process lifespan. Must be stateless or thread-safe.
    *   *The Trap:* **Captive Dependency.** Injecting a Scoped service (`DbContext`) into a Singleton leaks connection handles and causes runtime state corruption.

### Monolith to Microservices Transition
*   **Strategy:** Implement the **Strangler Fig Pattern** to break down legacy apps incrementally instead of a risky "Big Bang" rewrite.
*   **Execution:** 
    1. Define domain boundaries using Domain-Driven Design (DDD).
    2. Route all traffic through an **API Gateway** pointing initially to the monolith.
    3. Carve out a single domain into an independent .NET container with its own database schema.
    4. Repoint the gateway route to the new microservice. Repeat until the monolith is deprecated.

---

## 🔒 2. Security & Identity Management

### API Authentication & Authorization
*   **Implementation:** Enforce stateless **JWT (JSON Web Token) Bearer Authentication** middleware. The client presents a signed cryptographically verifiable token inside the HTTP authorization header.
*   **Microsoft Entra ID (Azure AD):** An identity-as-a-service provider. The client authenticates against Entra ID, which returns a JWT access token. Your .NET API uses the `Microsoft.Identity.Web` package to validate the token's cryptographic signature against Entra ID public keys without needing a database hit.
*   **Securing from Unauthorized/External Users:** 
    *   Enforce the `[Authorize]` attribute on API endpoints to block anonymous traffic.
    *   Implement **RBAC/CBAC** (Role/Claim-Based Access Control) to verify explicit scopes.
    *   Protect external network perimeters using an API Gateway configured with rate limiting, IP whitelisting, and Web Application Firewall (WAF) rule sets.

### Environment Connection String Isolation
*   **The Trap:** Storing secrets in cleartext files (`appsettings.json`) leaks credentials to source control and hazards data corruption if local tests target a live database tier.
*   **The Fix:** Leverage the `.NET Configuration Provider hierarchy` to separate environments:
    *   **Development:** Keep strings locally inside a `git-ignored` `appsettings.Development.json` or machine Secrets Manager tool.
    *   **QA & Production:** Store placeholders in code. Inject the actual connection values dynamically at container startup using **AWS Systems Manager Parameter Store/Secrets Manager** or **Azure App Service Environment Configurations**. The .NET configuration engine auto-overwrites keys seamlessly at runtime based on environment environment tokens.

---

## ☁️ 3. Cloud Infrastructure & Storage (Azure & AWS Terminology)

### App Deployment (Azure App Service / AWS Elastic Beanstalk)
*   **Process:** Package the .NET application into an immutable Docker image or compiled zip artifact via a CI/CD pipeline (e.g., GitHub Actions or AWS CodePipeline). Deploy using zero-downtime **Blue/Green or Canary patterns** at the load balancer layer to isolate release blast radiuses.

### Storage Selection: Blob vs. Table Storage
*   **Blob Storage (AWS S3 Equivalent):** An unmanaged object store built for heavy unstructured binary payloads (PDFs, legal briefs, images). Storing these here prevents transactional database index page bloat.
*   **Table Storage (AWS DynamoDB Equivalent):** A fast, low-cost NoSQL key-value store optimized for mass-volume structured telemetry datasets that don't need complex relational joins (like application access audit trails).
*   **Blob Storage Access Tiers:**
    *   **Hot:** High storage cost, zero retrieval cost. Best for immediate, high-frequency active files.
    *   **Cool:** Lower storage cost, minor retrieval penalty. Optimized for older documents accessed less than once a month.
    *   **Archive:** Microscopic storage cost, high retrieval penalty. Kept completely offline (requires up to several hours to rehydrate). Used strictly for statutory 7-year legal data retention compliance.

### Serverless & Messaging Integrations
*   **Azure Functions (AWS Lambda Equivalent):** Event-driven, serverless micro-compute blocks that scale horizontally to zero. Excellent for offloading atomic background utility loops (e.g., auto-generating a PDF preview thumbnail whenever a new case file hits storage) without paying for idle server uptime.
*   **Azure Service Bus (AWS SQS/RabbitMQ Equivalent):** A high-reliability enterprise message broker used to decouple asynchronous system processing queues. 
    *   *Why implement it:* Instead of processing a heavy legal document aggregation synchronously on an active HTTP thread, the API drops a lightweight JSON reference token into the broker queue and returns a `202 Accepted` status. Distributed backend background workers pull items from the bus matching their hardware capacity, preserving front-line API responsiveness.

---

## 🛠️ 4. Advanced Production Triage & Performance Tuning

### Live Production Debugging (API Fails on Deployment)
*   **The Trap:** Turning on detailed developer exception pages in a live production environment leaks core stack traces and schema layouts to external attackers.
*   **The 3-Step Triage Protocol:**
    1.  **Check Configuration Injection:** A crash instantly at container initialization indicates an invalid cloud environment parameter injection, missing credential, or an unmapped cloud IAM permission token wrapper.
    2.  **Inspect Live Telemetry Streams:** Query centralized log engines (**AWS CloudWatch / Azure Application Insights**) to isolate the explicit exception thrown during the root `Program.cs` composition startup step.
    3.  **Execute Synthetic Network Probes:** Run internal health check endpoints (`/health`) to confirm that the API web container can successfully traverse the secure internal cloud network perimeter (VPC) to talk to the database cluster.

### Resolving Post-Deployment Cloud Slowness
*   **Identification (Telemetry Analysis):** Pinpoint the exact bottleneck using **Distributed Tracing APM tools (Datadog / AWS X-Ray)**. Track the end-to-end latency waterfall chart of slow API requests to determine if the lag is sitting inside the compute layer, an external API connection, or a database query.
*   **The Remediation Matrix:**
    *   *If the Database is slow:* Run query execution plans to catch missing B-Tree indexes, resolve N+1 execution loops in your ORM layer, or apply `.AsNoTracking()` to read-only LINQ evaluations to clear Change Tracker heap load.
    *   *If the Compute layer is slow:* Profile for CPU/Memory constraints. Implement the **Cache-Aside Pattern** using an in-memory **Redis** cache to shield the database from identical repeating requests.
    *   *If Network IO is slow:* Offload static files to a Content Delivery Network (CDN) and compress API JSON transport payloads.
