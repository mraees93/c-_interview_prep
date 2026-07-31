# Consolidated System Design Patterns Cheat Sheet (Ordered by Data Journey)

| Problem / Failure Scenario | Common Solution | Senior Architecture Execution Details |
| :--- | :--- | :--- |
| **--- CATEGORY 1: TRAFFIC INGESTION & DATA FLOW ---** | | |
| **High-Write Traffic** | • Async Writes<br>• LSM-Tree Storage | Decouple ingestion via a message broker (RabbitMQ) to absorb write spikes. Use an LSM-Tree engine (like Cassandra) to write sequentially to memory first, bypassing standard disk indexing bottlenecks. |
| **Handling Massive Files** | • Object Storage Abstraction | Prevent database row and page bloat caused by giant strings or attachments. Store relational document metadata inside the transactional database, and offload the actual file binary data to an Object Store (S3 / Azure Blobs). |
| **Queue Payload Choking (Heavy Files in Broker)** | • Object Pointer Pattern | Never pass raw binary strings (PDFs, Images, massive text files) directly through message queues; it triggers extreme memory pressure. Save the heavy file to **AWS S3 first**, and drop a lightweight JSON metadata token containing the **S3 URL pointer** into RabbitMQ for the worker to pull. |
| **RabbitMQ Worker Ingestion Overload** | • Pull-Based Prefetch Tuning | To prevent background workers from being overwhelmed by a high-traffic spike, workers explicitly configure `BasicQos(PrefetchCount = 10)`. This forces the worker to actively **pull and process a capped allocation** of tasks asynchronously matching its physical CPU/RAM capacity. |
| **The "Poison Pill" Message** | • Dead Letter Queues (DLQ) | Prevent corrupt data payloads from locking up asynchronous worker pools in infinite retry loops. Configure the broker to automatically eject a message to an isolated DLQ after it hits a designated retry threshold. |
| **--- CATEGORY 2: COMPUTE & ASYNCHRONOUS PROCESSING ---** | | |
| **Third-Party API Downtime / Slowness** | • Asynchronous Task Workers | Isolate unpredictable external API integrations (taking 30s to 2m) away from the user HTTP request path. Drop tasks into a message bus and let independent, horizontally scalable background workers manage execution. |
| **Read-Heavy System** | • Use Caching<br>• Use Read Replicas | Implement the **Cache-Aside Pattern** with Redis. Serve lookups from memory and asynchronously populate the cache on a miss using non-blocking background tasks. Route heavy residual read traffic to read-only database replicas. |
| **Slow Full-Text Phrase Searches** | • Offload to Search Engine | Bypassing B-Tree database indexes for multi-keyword queries (e.g., legal document lookups). Mirror text data into an **Elasticsearch cluster** using inverted indexes to handle phrase searches under sub-second latencies. |
| **Data Inconsistency / Dual-Write Drift** | • Change Data Capture (CDC) | Avoid writing to two databases simultaneously from application code. Use a CDC engine (like Debezium) to tail primary transaction or replica logs asynchronously, streaming data deltas reliably into secondary search engines. **AKA Eventual Consistency via CDC**. |
| **--- CATEGORY 3: DATABASE STRUCTURE & HORIZONTAL SCALING ---** | | |
| **Slow Database Queries** | • Indexing & Optimization | Analyze database execution plans. Apply target B-Tree indexes on highly filtered columns (`WHERE` clauses), optimize complex table joins, and continuously update database statistics to prevent sequential table scans. |
| **Relational Index Bloat** | • Range Table Partitioning | Prevent giant B-Tree index degradation over tens of millions of rows. Segment a single monolithic table into distinct time-based chunks like smaller, manageable sub-tables (e.g., monthly partitions) on the same host, keeping active write trees compact. |
| **DB too big for single machine's storage or RAM** | 1. Write-heavy => Hash-based Sharding<br> 2. Read-heavy => Range-Based Sharding | 1. You hash the ID to scatter data randomly across servers. It is perfect for high-volume writes because it prevents bottlenecks, but bad for searching data ranges.<br> 2. You group data by ranges (like dates or numbers) on specific servers. It is perfect for fast range scans, but highly vulnerable to hotspots if all new traffic hits today's date range. |
| **Database Shard Imbalance (Hot Shards)** | • Composite Key Sharding | Avoid sharding solely on generic keys (like `Region_ID`) if one cluster segment receives 80% of data. Mix the shard layout by implementing a composite key (e.g., `Region_ID + Calendar_Year`) to spread data evenly. |
| **--- CATEGORY 4: SECURITY, LEDGERS & FAULT TOLERANCE ---** | | |
| **Rogue Root Administrator Modifying Historical Logs** | • Application-Level Hash Chain | Infrastructure defenses (VPCs, mTLS, RBAC) fail if an attacker compromises data with root credentials. Enforce structural integrity by computing an application-level **Cryptographic Hash Chain** on every write: `Hash(Current Log Data + Hash of Prior Row)`. Any low-level alteration fractures the mathematical continuity, causing compliance verification scripts to instantly fail. |
| **Ledger Primary Key Write Bottlenecks at Scale** | • Tenant Chain Segmentation | Forcing 50 million monthly audit logs into a single, global linear hash chain causes extreme row-locking contention because every write must read the preceding row. Eliminate lock overhead by **segmenting the hash chains by Tenant ID or Document UUID**. This spins up thousands of concurrent micro-ledgers that execute fast and scale horizontally. |
| **Reconstructing Corrupted Data After a Security Breach** | • WORM (Write-Once-Read-Many) Archiving | If a database layer is altered or corrupted, a hash chain detects the breach but cannot fix the data. Back your logging tier by streaming raw transactional payloads simultaneously into a cold storage tier (like an **AWS S3 Bucket with Object Lock enabled** in Compliance Mode). This acts as an immutable, permanent legal baseline to execute full disaster recovery and system audits. |
| **Single Point of Failure (SPOF)** | • Redundancy & Standby Nodes | Eliminate isolated hardware choke points. Configure a live, synchronous **Hot Standby Replica** coupled with an automated cluster health monitor to execute instant failover traffic routing if the primary node dies. |
| **Internal Network Packet Sniffing** | • Zero-Trust Architecture | Assume the internal cloud network perimeter is hostile. Enforce strict Mutual TLS (**mTLS**) for all inner-service and database connections, and replace static credentials with short-lived **ephemeral IAM tokens**. |

***

## The "Why Choose RabbitMQ Over Kafka?" Interview Pivot Rule

If an interviewer pushes you on tool selection (*"Why use RabbitMQ instead of Kafka?"*), use this concise strategic framework to defend your choices cleanly:

*   **When to defend RabbitMQ (Your Default Choice):** It is the standard industry pattern for handling complex routing, targeted task execution, and varying background jobs (like webhooks or slow 3-minute REST integration tasks). It keeps your architecture simple by eliminating partition management math, consumer rebalancing, and client-side offset complexity.
*   **When to switch to Kafka (The Exception):** Only mention Kafka if the interviewer introduces explicit architectural requirements stating that the data ingestion velocity exceeds **100,000 streaming events per second**, or if the system requires **7-day stream rewind capabilities** to re-process identical event histories out-of-order.

## **When to use Table partitioning VS when to use DB sharding?**
**Think vertical scaling VS horizontal scaling**
Use Table Partitioning to split a massive table onto a single server to prevent local index bloat and keep active data cached in RAM. Transition to Database Sharding when data scale or write traffic breaks the physical hardware limits of a single machine, forcing you to distribute rows across multiple separate servers over the network.

With partitioning, the database engine handles the routing seamlessly. With sharding, the application layer or a router middleware has to know exactly which physical machine holds the data.

## Brokers, Queues, and Workers:
### How does Messaging Architecture work? 

This reference summary defines the differences and interactions between message brokers, message queues, and asynchronous workers.

---

### 1. The Component Definitions
* **Message Broker:** A complete middleware software system (e.g., RabbitMQ, Apache Kafka, Azure Service Bus) that manages, routes, validates, and maintains one or multiple message queues.
* **Message Queue:** A sequential data structure that holds data packets/messages in order (typically FIFO — First In, First Out) until they can be safely processed.
* **Async Worker:** A background computing process (e.g., a .NET `BackgroundService`, AWS Lambda, or Hangfire worker) that actively listens to a queue, pulls messages out, and executes the actual business logic.

---

### 2. The Key Differences
* **Data vs. System vs. Compute:** the broker is the **infrastructure server** orchestrating the data, the queue is the **storage container**, and the worker is the **processing engine**.
* **Responsibility Split:** The broker ensures messages are reliably routed, stored, and delivered; the worker ensures the heavy processing (e.g., generating PDFs, sending emails) happens asynchronously without blocking the main web application/UI thread.

---

### 3. How They Work Together (The Lifecycle Flow)
* **Step A:** Your main application (e.g., an API Controller) sends a payload (like `"Send Welcome Email to User 123"`) to the **Message Broker**.
* **Step B:** The **Message Broker** processes the routing rules, finds the destination, and places the payload safely inside the designated **Message Queue**.
* **Step C:** An idle **Async Worker** pulls the message from that **Message Queue**, processes the email in the background, and tells the broker to remove the message from the queue upon successful completion.

---

# Reference: Read/Write Scaling Techniques & Async Workers

### 1. Scaling Comparison Matrix

| Load Profile | Primary Scaling Technique | Async Worker Strategy | Core Focus |
| :--- | :--- | :--- | :--- |
| **Read-Heavy** | **Horizontal Read Replication** & Caching | **Cache-Invalidation / Warming Workers** | Offloading database read I/O |
| **Write-Heavy** | **Queue-Based Load Leveling** (Buffering) | **Batch-Processing Workers** | Smoothing traffic spikes & bulk inserts |

---

### 2. Summary of Techniques

#### Read-Heavy Workloads
* **The Technique:** Horizontal scaling via **Read Replicas** combined with distributed caching (Redis). 
* **The Async Worker Role:** Workers operate on the *output* side of data changes. When data is modified, async workers run in the background to invalidate old cache keys or pre-calculate (warm) heavy search indexes, ensuring read paths stay fast.

#### Write-Heavy Workloads
* **The Technique:** Horizontal scaling via **Message Queues** to buffer incoming traffic, decoupling the API ingestion layer from the database storage tier.
* **The Async Worker Role:** Workers operate on the *input* side of data changes. Instead of processing incoming writes one-by-one, async workers pull batches of messages from the queue and perform highly optimized bulk database inserts to minimize lock contention.

# Reference: Worker Scaling for Traffic Spikes

### Worker Scaling Matrix

| Scaling Type | Action Taken | Primary Use Case | Interview Key Phrase |
| :--- | :--- | :--- | :--- |
| **Horizontal Scaling** | Add **more worker instances** (containers/clones) | Handling sudden **traffic spikes** & queue backlogs | "Scale out based on **Queue Depth**" |
| **Vertical Scaling** | Add **CPU/RAM** or increase internal C# task counts | Setting the **baseline capacity** for a single instance | "Optimize resource limits per process" |

---

### 3 Quick Rules to Remember
1. **Spikes = Horizontal:** When messages pile up during a spike, clone the workers horizontally to process the queue faster.
2. **Monitor the Queue, Not CPU:** Trigger horizontal autoscaling using **Queue Depth** (the number of backlogged messages), not server CPU metrics.
3. **Always Run 2+ Minimum:** Even under low traffic, always run at least 2 horizontal instances to ensure high availability if one worker crashes.

---

# Hybrid Scaling: Switching & Combining Vertical and Horizontal Strategies

While hyper-scale applications stay permanently horizontal, most standard enterprise systems strategically switch between or combine vertical and horizontal scaling depending on the specific component, environment, or lifecycle phase.

---

### 1. Hybrid Scaling Scenarios (The Switch & Combine Matrix)

| Operational Scenario | Component Action | Scaling Strategy Used | Architectural Reason |
| :--- | :--- | :--- | :--- |
| **Planned High-Traffic Event**<br>*(e.g., Flash sale, Black Friday)* | **Database Layer:** Vertical<br>**Worker Layer:** Horizontal | **Combined Scaling** | Databases handle writes best when centralized to avoid distributed transaction lockups. Workers scale out infinitely to process the input queue. |
| **Architectural Migration**<br>*(App growth over time)* | **Day 1 Monolith:** Vertical<br>**Day N Microservices:** Horizontal | **Lifecycle Switching** | Start vertically by upsizing a single server to maximize speed. Switch horizontally later when business domains split. |
| **Resource Bottleneck Shift**<br>*(From CPU-bound to I/O-bound)* | **Phase 1 (Math/Crypto):** Vertical<br>**Phase 2 (API/Network):** Horizontal | **Dynamic Adaptation** | Heavy computational steps require faster single-core CPUs (Vertical). Network-bound tasks prefer distributed clones (Horizontal). |

---

### 2. Strategic Rules for the Interview

* **Scale the Tier, Not the App:** Never assume the entire application must scale the same way. You can scale your C# web API and async workers **horizontally** while keeping your SQL Server database scaled **vertically** on a massive cloud instance to preserve instant ACID transactions.
* **The Migration Switch:** The transition from vertical to horizontal scaling is the classic hallmark of modernizing an application. Upgrading a server vertically is the fastest short-term fix for traffic growth, but migrating to horizontal scaling is the only sustainable long-term solution for true system elasticity.

---

### LexisNexis-Specific Scenario Matrix

| Interview Scenario Prompt | The Engineering Choke Point | The Correct Architectural Response |
| :--- | :--- | :--- |
| **"We ingest 100,000 new court case files in a nightly batch. How do we update our system without knocking our active user web servers offline?"** | Batch processing jobs draining transactional database connections and CPU. | **Decoupled Bulk Ingestion:** Read files into an isolated staging data pipeline. Use bulk-insert scripts during off-peak hours on the Primary node, or use independent background batch jobs that stream data via Kafka partitions. |
| **"Multiple law firms are using our search platform. How do we ensure Firm A cannot see Firm B's saved search histories or automated email alerts?"** | Multi-tenant data isolation and protection against data leakage. | **Logical Partitioning & Tenant Filters:** Implement Row-Level Security (RLS) in PostgreSQL or enforce a tenant-ID discriminator key (`Tenant_ID`) on every database query, cache key, and Elasticsearch filter. |
| **"Our legal datasets rarely change after they are written, but our users read them constantly. Our database costs are spiking. How do we fix this?"** | Over-provisioning expensive primary database compute for historical, read-only data. | **Tiered Storage Architecture:** Move historical, cold case records to cheap Object Storage (S3/Azure Blobs). Keep active, recent metadata in PostgreSQL, and cache high-frequency landing queries in Redis. |
| **"An external government API we depend on has strict rate limits. If our workers call it too fast, they block our IP address. How do we handle this?"** | Uncontrolled consumer worker pools overwhelming down-stream third-party APIs. | **Token Bucket Rate Limiting / Throttling:** Implement a token-bucket rate limiter middleware on your RabbitMQ consumers. Restrict the number of active worker threads (Concurrency Limits) to match the external provider's Max-RPS allowance. |