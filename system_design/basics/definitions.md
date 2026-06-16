# System Design Principles: The Request-Response Lifecycle

> **Quick Summary:** Use **Bucket 1** to get them in, **Bucket 2** to make it fast, and **Bucket 3** to make sure it never breaks.

---

### BUCKET 1: The "Traffic and Protection" Bucket
**Goal:** Manage how requests enter the system and ensure no server gets crushed.


| Concept | What | When to Use | Why (Benefit) | Trade-off (The Catch) | Failure Impact / Notes |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Load Balancer** | A traffic cop that sits in front of your servers. | When one server cannot handle traffic or you need high availability. | Spreads the load; if one server dies, the system stays up. | Adds a mandatory "middleman" layer to infrastructure. | **If it fails:** Absolute system outage unless a backup balancer is configured. |
| **Rate Limiter** | A throttle that limits how many requests a user can make. | When you need to prevent API abuse, scraping, or DDoS attacks. | Protects backend resources and saves massive cloud scaling costs. | Misconfigurations can frustrate and block valid users. | **If it fails:** Downstream services get flooded, leading to database crashes. |
| **API Gateway** | A single entry point that handles routing, authentication, and logging. | In microservices where you have dozens of different background services. | Simplifies client logic and centralises authentication/security in one place. | Introduces high operational complexity and architectural overhead. | **If it fails:** Clients cannot talk to microservices; becomes a total point of failure. |
| **DNS** | A system that translates human domains into machine IP addresses. | Every time a client initiates an internet request to your system. | Routes users to the nearest regional entry point or load balancer automatically. | Updates take time to propagate globally due to aggressive caching. | **If it fails:** The application becomes entirely unreachable by name worldwide. |

---

### BUCKET 2: The "Performance and Speed" Bucket
**Goal:** Reduce latency and make the application feel "instant" for the user.


| Concept | What | When to Use | Why (Benefit) | Trade-off (The Catch) | Failure Impact / Notes |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Caching** | High-speed temporary data storage (like Redis). | When you have hot data that is read frequently (e.g., viral tweet). | Massive speed boost; takes heavy read load off your database. | Data staleness. Cache may show old data if the database updates. | **Cache Stampede:** If cache clears, database gets crushed by sudden traffic. |
| **CDN** | A globally distributed network of edge caching servers. | When users are worldwide and you have static files (images, PDFs, UI assets). | Eliminates physical network latency by serving files close to the user. | High financial costs; cache propagation delays during system updates. | **If it fails:** Latency spikes globally as traffic drops back to origin servers. |
| **SQL vs. NoSQL** | Relational data structures (Postgres) vs. Non-relational (MongoDB). | SQL for complex relationships; NoSQL for massive scale/simple datasets. | **SQL:** Data integrity (ACID).<br>**NoSQL:** High write speeds and easy scaling. | **SQL:** Hard to scale horizontally.<br>**NoSQL:** Lacks native, complex ACID joins. | Choosing the wrong model results in massive structural re-engineering mid-project. |
| **Consistent Hashing** | A distributed hashing strategy mapping keys to a virtual ring structure. | When scaling distributed caches or databases dynamically. | Minimizes data reshuffling and re-mapping overhead when changing node membership. | Complex implementation; requires virtual nodes to avoid uneven hotspots. | **Without it:** Adding one cache node invalidates nearly 100% of your current cache keys. |
| **APIs<br>(REST vs. gRPC)** | Interface protocols defining how services exchange network data payloads. | **REST:** Public web APIs/CRUD.<br>**gRPC:** High-performance internal services. | **REST:** Universal compatibility.<br>**gRPC:** Compact binary transport, extreme speed. | **REST:** Large payload size, slow text parsing.<br>**gRPC:** Hard to debug, poor native web support. | Using REST for internal microservices leads to high internal latency under load. |
| **Polling vs.<br>WebSockets** | Communication models determining client data-fetching frequency. | **Polling:** Low frequency updates.<br>**WebSockets:** Real-time bi-directional streams. | **Polling:** Simple setup.<br>**WebSockets:** Near-zero latency payload transport. | **Polling:** Huge server overhead.<br>**WebSockets:** Difficult to scale and load balance. | Using standard polling for real-time applications quickly exhausts server ports. |
| **Cache Invalidation** | Rules defining when cached data is old and must be deleted. | Every single time you implement a caching layer in your architecture. | Guarantees users do not view stale, outdated, or incorrect information. | Noticeably difficult to get right without introducing race conditions. | Out-of-sync legal data or user permissions can create compliance risks. |
| **DB Indexing** | A data structure (like a B-Tree) that speeds up data retrieval. | On database columns that are frequently targeted in search queries. | Drastically drops query latency by avoiding slow, full-table scans. | Slows down data writes (`INSERT`/`UPDATE`) and consumes extra disk space. | Missing indexes cause database CPU usage to hit 100% under mild query load. |

---

### BUCKET 3: The "Scale and Reliability" Bucket
**Goal:** Ensure the system can grow to millions of users without losing data or crashing.


| Concept | What | When to Use | Why (Benefit) | Trade-off (The Catch) | Failure Impact / Notes |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Replication** | Keeping copies of the same data on multiple database servers. | When you need read scalability or want to ensure data isn't lost if a disk fails. | If the primary database dies, a read-follower can take over operations. | Sync delay (Replication Lag) introduces temporary data inconsistencies. | **If it fails:** Data loss occurs if the primary drive experiences physical corruption. |
| **Sharding** | Splitting a large dataset into smaller "shards" across many databases. | When your database is too big for a single machine's storage or RAM. | Enables theoretical infinite horizontal scaling for your data tiers. | Makes transactional joins across different database shards incredibly complex. | **Hotspotting:** Poor shard key selection makes one database machine do all the work. |
| **Message Queue** | An asynchronous task inbox (Kafka, RabbitMQ, Service Bus). | When a task takes a long time (e.g., extracting text, generating PDFs). | Decouples services so Service A doesn't have to wait for Service B to finish. | Significantly harder to debug, track distributed traces, and monitor. | **Queue Backlog:** If workers die, messages stack up, delaying updates indefinitely. |
| **CAP Theorem** | A model stating distributed systems only pick 2 of: C, A, or P. | Every single time you architect a distributed system. | Forces intentional trade-offs between "always correct" vs. "always online". | Networks will partition; you must explicitly choose to sacrifice C or A. | Over-promising both leads to split-brain systems and corrupted data states. |
| **Scaling<br>(Horiz. vs Vert.)** | Infrastructure expansion by adding more boxes (Horiz.) or larger CPUs (Vert.). | **Horizontal:** Distributed systems.<br>**Vertical:** Early-stage architectures/DBs. | **Horizontal:** No ceiling limit.<br>**Vertical:** Zero application code changes required. | **Horizontal:** Apps must be stateless.<br>**Vertical:** Strict physical hardware limit. | Relying purely on vertical scaling creates an expensive single point of failure. |
| **Failover** | An automated backup routine switching traffic to standby nodes upon crash. | Critical architectures where unplanned database or service downtime causes loss. | Ensures near-constant system availability and continuous uptime during crashes. | High financial cost for idle backup servers; risk of data loss during cutover. | Manual failover routines guarantee elongated outages during off-hours. |
| **Circuit Breaker** | A pattern that blocks requests to a microservice that is currently failing. | When calling unstable downstream dependencies or external microservices. | Prevents a single sluggish service from dragging down and freezing your entire app. | Complex to mock, implement, and thoroughly test. | Without it, a slow third-party API will back up threads and crash the system. |
