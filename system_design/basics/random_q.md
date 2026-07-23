# Whats a bottleneck?
A bottleneck is a single component or resource that limits the capacity, speed, or throughput of your entire application.
Even if every other part of your system is lightning-fast, the system can only perform as fast as its slowest component (the bottleneck).

* **Definition:** The slowest component in a system that limits total performance, throughput, or speed.
* **The Golden Rule:** Optimizing non-bottleneck components yields zero overall performance gain. You must find and fix the specific bottleneck.
* **Common Examples:** Maxed-out CPU, slow disk drives (IOPS exhaustion), unindexed database tables, and blocking third-party API dependencies.


# How do I know when we hit the hardware limit for a SQL or NoSQL DB?

### Database Hardware Limits Master Reference (SQL vs. NoSQL)

| Metric / Signal Category | SQL Databases (e.g., PostgreSQL) | NoSQL Databases (e.g., Cassandra) |
| :--- | :--- | :--- |
| **Shared: CPU Saturation** | Usage hovers above 80%–90% continuously. | Usage hovers above 80%–90% continuously. |
| **Shared: Disk I/O Bottleneck** | IOPS exhausted; high I/O wait times. | IOPS exhausted; high I/O wait times. |
| **Shared: Application Distress** | p99 query latency spikes; backend connection timeouts. | p99 query latency spikes; backend connection timeouts. |
| **Primary System Bottleneck** | **Memory (RAM) & Disk IOPS** | **CPU & Disk I/O during Compaction** |
| **Key Infrastructure Metric** | **Cache Hit Ratio drops below 99%** (Indexes no longer fit in RAM, forcing slow disk reads). | **Compaction Pending Tasks climb** (System cannot clean up old disk files fast enough). |
| **Write Failure Mode** | **Lock Contention / Connection Pool Exhaustion** (Queries queue up waiting for row/table locks). | **CommitLog/Memtable Saturation** (Node starts dropping incoming writes or throwing write timeouts). |
| **Application Symptom** | **p99 Latency spikes** on complex queries and relational table joins. | **Read Latency spikes** because the system must check too many un-compacted SSTables. |
| **Storage Failure** | **Physical Disk Capacity** reached on the single primary database instance. | **Data Replication Lag** and disk imbalance across the cluster nodes. |




# When to introduce sharding to horizontally scale?
when we have already implemented Range Table Partitioning for vertical scaling and we now hitting the hardware limit, then only we can introduce any of the 2 sharding techniques.

# whats a stateless app?
A stateless app is a system where the server does not store any user data, history, or session context between requests. Every request from a client must be completely self-contained, containing all the information required for the server to process it.

Benefits:

Horizontal Scaling: You can instantly add or remove servers to handle traffic spikes, since any server can process any incoming request.
High Availability: If a server crashes, users can be seamlessly rerouted to a healthy one without losing their session progress.
Lower Server Costs: The server consumes less memory because it does not need to store and maintain millions of active user sessions.
Simpler Maintenance: Deploying updates is easier because you can restart or replace servers without needing to migrate live session data.

Trade-offs

Larger Network Payloads: Because the server remembers nothing, the client must send authentication tokens and context data with every single request, increasing bandwidth usage.
Client-Side Complexity: The frontend application (browser or mobile app) bears the responsibility of securely storing tokens and managing application state.
Database Heavy: Since the server doesn't hold data in local memory, it must frequently query databases or external caches (like Redis) to verify permissions or retrieve user records.

# what's Operational Overhead?

refers to the ongoing time, money, and human effort required to keep a software system running smoothly, safely, and reliably in production.

e.g. LLM-TriageAgent would be high operational overhead if i replaced polling with websockets

# Whats High-throughput?

High-throughput refers to a system's ability to process a massive volume of data or requests within a specific timeframe. While "speed" usually refers to how fast a single task finishes (latency), throughput refers to how many tasks are completed altogether.

Think of high-throughput like a 10-lane highway:It is not built to make a single sports car travel faster.It is built to move thousands of trucks past a checkpoint every hour.

Benefits:
Massive Concurrency: The system handles thousands or millions of concurrent users without crashing.