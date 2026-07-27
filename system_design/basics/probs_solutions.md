### Common System Design Problems and Solutions

| Problem / Failure Scenario | Common Solution | Senior Architecture Execution Details |
| :--- | :--- | :--- |
| **Read-Heavy System** | • Use Caching<br>• Use Read Replicas | Implement the **Cache-Aside Pattern** with Redis. Serve lookups from memory and asynchronously populate the cache on a miss using non-blocking background tasks. Route heavy residual read traffic to read-only database replicas. |
| **High-Write Traffic** | • Async Writes<br>• LSM-Tree Storage | Decouple ingestion via a message broker (RabbitMQ/Kafka) to absorb write spikes. Use an LSM-Tree engine (like Cassandra) to write sequentially to memory first, bypassing standard disk indexing bottlenecks. |
| **Slow Full-Text Phrase Searches** | • Offload to Search Engine | Bypassing B-Tree database indexes for multi-keyword queries (e.g., legal document lookups). Mirror text data into an **Elasticsearch cluster** using inverted indexes to handle phrase searches under sub-second latencies. |
| **Single Point of Failure (SPOF)** | • Redundancy & Standby Nodes | Eliminate isolated hardware choke points. Configure a live, synchronous **Hot Standby Replica** coupled with an automated cluster health monitor to execute instant failover traffic routing if the primary node dies. |
| **Data Inconsistency / Dual-Write Drift** | • Change Data Capture (CDC) | Avoid writing to two databases simultaneously from application code. Use a CDC engine (like Debezium) to tail primary transaction or replica logs asynchronously, streaming data deltas reliably into secondary search engines. **AKA Eventual Consistency via CDC** |
| **The "Poison Pill" Message** | • Dead Letter Queues (DLQ) | Prevent corrupt data payloads from locking up asynchronous worker pools in infinite retry loops. Configure the broker to automatically eject a message to a isolated DLQ after it hits a designated retry threshold. |
| **Relational Index Bloat** | • Range Table Partitioning | Prevent giant B-Tree index degradation over tens of millions of rows. Segment a single monolithic table into distinct time-based chunks like smaller, manageable sub-tables (e.g., monthly partitions) on the same host, keeping active write trees compact. |
| **DB too big for single machine's storage or RAM** | 1. Write-heavy => Hash-based Sharding (Consistent Hashing)<br> 2. Read-heavy => Range-Based Sharding | 1. You hash the ID to scatter data randomly across servers. It is perfect for high-volume writes because it prevents bottlenecks, but bad for searching data ranges.<br> 2.  You group data by ranges (like dates or numbers) on specific servers. It is perfect for fast range scans, but highly vulnerable to hotspots if all new traffic hits today's date range. |
| **Database Shard Imbalance (Hot Shards)** | • Composite Key Sharding | Avoid sharding solely on generic keys (like `Region_ID`) if one cluster segment receives 80% of data. Mix the shard layout by implementing a composite key (e.g., `Region_ID + Calendar_Year`) to spread data evenly. |
| **Internal Network Packet Sniffing** | • Zero-Trust Architecture | Assume the internal cloud network perimeter is hostile. Enforce strict Mutual TLS (**mTLS**) for all inner-service and database connections, and replace static credentials with short-lived **ephemeral IAM tokens**. |
| **Slow Database Queries** | • Indexing & Optimization | Analyze database execution plans. Apply target B-Tree indexes on highly filters columns (`WHERE` clauses), optimize complex table joins, and continuously update database statistics to prevent sequential table scans. |
| **Handling Massive Files** | • Object Storage Abstraction | Prevent database row and page bloat caused by giant strings or attachments. Store relational document metadata inside the transactional database, and offload the actual file binary data to an Object Store (S3 / Azure Blobs). Stores unstructured data (like images and videos) as standalone "objects" or "blobs" with unique IDs and metadata |
| **Third-Party API Downtime / Slowness** | • Asynchronous Task Workers | Isolate unpredictable external API integrations (taking 30s to 2m) away from the user HTTP request path. Drop tasks into a message bus and let independent, horizontally scalable background workers manage execution. |



## **When to use Table partitioning VS when to use DB sharding?**
**Think vertical scaling VS horizontal scaling**
Use Table Partitioning to split a massive table onto a single server to prevent local index bloat and keep active data cached in RAM. Transition to Database Sharding when data scale or write traffic breaks the physical hardware limits of a single machine, forcing you to distribute rows across multiple separate servers over the network.

With partitioning, the database engine handles the routing seamlessly. With sharding, the application layer or a router middleware has to know exactly which physical machine holds the data.

## Brokers, Queues, and Workers:
### How does Messaging Architecture work? 

This reference summary defines the differences and interactions between message queues, message brokers, and asynchronous workers.

---

### 1. The Component Definitions
* **Message Queue:** A sequential data structure that holds data packets/messages in order (typically FIFO — First In, First Out) until they can be safely processed.
* **Message Broker:** A complete middleware software system (e.g., RabbitMQ, Apache Kafka, Azure Service Bus) that manages, routes, validates, and maintains one or multiple message queues.
* **Async Worker:** A background computing process (e.g., a .NET `BackgroundService`, AWS Lambda, or Hangfire worker) that actively listens to a queue, pulls messages out, and executes the actual business logic.

---

### 2. The Key Differences
* **Data vs. System vs. Compute:** The queue is the **storage container**, the broker is the **infrastructure server** orchestrating the data, and the worker is the **processing engine**.
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

### LexisNexis-Specific Scenario Matrix

| Interview Scenario Prompt | The Engineering Choke Point | The Correct Architectural Response |
| :--- | :--- | :--- |
| **"We ingest 100,000 new court case files in a nightly batch. How do we update our system without knocking our active user web servers offline?"** | Batch processing jobs draining transactional database connections and CPU. | **Decoupled Bulk Ingestion:** Read files into an isolated staging data pipeline. Use bulk-insert scripts during off-peak hours on the Primary node, or use independent background batch jobs that stream data via Kafka partitions. |
| **"Multiple law firms are using our search platform. How do we ensure Firm A cannot see Firm B's saved search histories or automated email alerts?"** | Multi-tenant data isolation and protection against data leakage. | **Logical Partitioning & Tenant Filters:** Implement Row-Level Security (RLS) in PostgreSQL or enforce a tenant-ID discriminator key (`Tenant_ID`) on every database query, cache key, and Elasticsearch filter. |
| **"Our legal datasets rarely change after they are written, but our users read them constantly. Our database costs are spiking. How do we fix this?"** | Over-provisioning expensive primary database compute for historical, read-only data. | **Tiered Storage Architecture:** Move historical, cold case records to cheap Object Storage (S3/Azure Blobs). Keep active, recent metadata in PostgreSQL, and cache high-frequency landing queries in Redis. |
| **"An external government API we depend on has strict rate limits. If our workers call it too fast, they block our IP address. How do we handle this?"** | Uncontrolled consumer worker pools overwhelming down-stream third-party APIs. | **Token Bucket Rate Limiting / Throttling:** Implement a token-bucket rate limiter middleware on your RabbitMQ consumers. Restrict the number of active worker threads (Concurrency Limits) to match the external provider's Max-RPS allowance. |