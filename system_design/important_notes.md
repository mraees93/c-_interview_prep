# LexisNexis Interview Mastery Guide
**Target Role:** Intermediate Software Engineer (Cape Town)  
**Focus:** System Design, Architectural Defense, & Technical Panels

---

## 1. System Design Self-Practice Blueprint

To continue getting better at system design on your own, do not try to memorize specific architectures. Instead, practice the **Three-Step Deconstruction Method** on blank Excalidraw canvases.

### Step 1: Establish the "Happy Path" (First 10 Minutes)
Draw a clean, left-to-right architecture that fulfills only the core requirements under perfect conditions.
*   **Left:** Users, mobile/web clients, Load Balancer.
*   **Middle:** API Gateway, microservices, asynchronous message brokers.
*   **Right:** Relational/Non-relational databases, caches, and third-party APIs.

### Step 2: Play the Villain (The "What-If?" Matrix)
Look at your clean diagram and actively try to break it. Go component by component and ask yourself:
*   **Network Failures:** What happens if the third-party API slows down from 200ms to 2 minutes or goes offline entirely?
*   **Hardware Crashes:** What if the primary database server physically loses power?
*   **Data Anomalies:** What if a client submits corrupt data that crashes a backend worker thread?
*   **Traffic Spikes:** What if traffic scales up 100x tomorrow morning? Which component chokes first?

### Step 3: Apply the Architectural Patch
Evolve your diagram by introducing production-grade resilience patterns to solve the failures you discovered in Step 2:
*   Isolate slow APIs with **Message Queues (RabbitMQ/Kafka)** and worker pools.
*   Protect workers from corrupt data loops using **Dead Letter Queues (DLQs)**.
*   Prevent data loss and dual-write drift using **Change Data Capture (CDC)** engines tracking replica logs.
*   Shield database CPUs from heavy repeated reads using a **Cache-Aside Pattern (Redis)**.

---

## 2. Technical Panel Interview Framework

During the actual interview at LexisNexis, you are being judged more on *how you think* than on finding a single perfect answer. Follow this structured conversation framework to drive the session.

### Phase 1: Clarify and Scope (First 5 Minutes)
Never start drawing immediately. It is an instant red flag. Spend the first 5 minutes gathering requirements to define the boundaries of the system.
*   **Ask about scale:** *"What is our total data volume (e.g., millions or billions of rows)?"*
*   **Ask about traffic:** *"What is our peak throughput (e.g., requests per second)?"*
*   **Ask about expectations:** *"Is this system read-heavy (like search engines) or write-heavy (like audit logging)?"*

### Phase 2: Talk While You Draw
Silence is an interview killer. As you place boxes on your canvas, narrate your exact thought process out loud.
*   **Say this:** *"I am placing RabbitMQ here because our third-party provider is slow. By placing a queue between our gateway and the worker, we decouple the system and prevent our user threads from locking up."*

### Phase 3: Proactively Call Out Trade-offs
Every single technical choice has a downside. Do not wait for the interviewer to find it—point it out yourself to demonstrate senior-level foresight.
*   **Say this:** *"I chose Cassandra here because we need massive write speeds. However, I know the trade-off is eventual consistency, meaning a user might read slightly stale data for a brief window before the nodes sync."*

### Phase 4: State Your Limit Authentically
If the panel pushes you into a deep technical corner where you don't know the answer, do not guess or bluff. Be transparent, state your architectural instinct, and ask for collaboration.
*   **Say this:** *"I haven't implemented mTLS configurations manually at the infrastructure network layer in my previous role, but my architectural instinct tells me we need to handle token encryption over the wire here to prevent internal packet sniffing. How does your team typically handle this setup?"*

---

## 3. High-Yield "Tips & Tricks" Cheat Sheet

| Category | The Trap (What Candidates Do Wrong) | The Trick (How to Stand Out) |
| :--- | :--- | :--- |
| **Databases** | Storing everything in standard relational tables out of habit. | **Separate Metadata from Blobs:** Store light data fields in SQL, and push heavy files (PDFs, text) to S3/Azure Blob Storage. |
| **Syncing** | Suggesting "dual-writing" from the app backend to sync two databases. | **Use Eventual Consistency via CDC:** Explain how a tool like Debezium reads transaction logs asynchronously to keep things fast. |
| **Queues** | Relying on simple database flags to act as a task manager queue. | **Use Dedicated Brokers:** Use RabbitMQ for complex task routing, and Apache Kafka for high-throughput streaming/retention. |
| **Scaling** | Instantly jumping to complex cross-server sharding for medium tables. | **Use Table Partitioning First:** Explain that range partitioning by date on a single instance keeps indexes light and saves costs. |
| **Caching** | Caching every single database query without expiration rules. | **Eviction & TTL Policies:** Always state your cache Time-To-Live (TTL) strategy and how you handle cache invalidation on updates. |



## 4. The 6 Core Architectural Cards

If you get stuck or feel overwhelmed during a live scenario-based interview question, do not try to remember every complex pattern. Instead, reach for one of these six fundamental cards in your pocket to solve almost any infrastructure problem.

### 🛡️ Card 1: The Shield (Redis Cache)
*   **When to use it:** Slowness, high database CPU usage, or heavy repeated read queries.
*   **The Interview Answer:** *"I will drop a Redis cache in front of the service using a Cache-Aside pattern. We check memory first, and on a cache miss, we read from the database and backfill Redis with a set Time-To-Live (TTL)."*

### 🪵 Card 2: The Shock Absorber (RabbitMQ / Kafka)
*   **When to use it:** Heavy traffic spikes, unpredictable workloads, slow third-party APIs, or downstream service crashes.
*   **The Interview Answer:** *"I will decouple these services using an asynchronous message queue. This allows our backend to ingest requests instantly and lets background workers process tasks at a steady pace. If a service crashes, messages wait safely on the disk log."*

### 👥 Card 3: The Copy (Database Replicas)
*   **When to use it:** Hardware failures, high availability requirements, or data reporting slowing down production.
*   **The Interview Answer:** *"I will set up database replication. We will route all heavy reporting or search operations to read replicas to keep the primary node free, and maintain a hot standby replica with automated failover in case the main server dies."*

### 🔍 Card 4: The Sorter (Elasticsearch)
*   **When to use it:** Users performing complex keyword, wildcard, fuzzy text, or multi-phrase lookups (common for legal document searches).
*   **The Interview Answer:** *"Relational database B-Tree indexes struggle with full-text keyword searches across millions of documents. I will mirror our text data into Elasticsearch to leverage inverted indexing for sub-second search responses."*

### 🔪 Card 5: The Splitter (Table Partitioning)
*   **When to use it:** Relational database indexes bloating and slowing down writes because a table is receiving tens of millions of rows a month (like audit logs).
*   **The Interview Answer:** *"Instead of full database sharding which adds server complexity, I will use Range Table Partitioning by Date on the local instance. This splits one massive table into small monthly chunks so the database only updates a lightweight index tree."*

### 💂 Card 6: The Guard (mTLS & Identity Tokens)
*   **When to use it:** Protecting sensitive compliance or user data from internal network packet sniffing or container breaches.
*   **The Interview Answer:** *"I will enforce a Zero-Trust network architecture. We will encrypt all service-to-service communication over the wire using Mutual TLS (mTLS) and replace hardcoded configuration passwords with ephemeral, short-lived IAM database authentication tokens."*

