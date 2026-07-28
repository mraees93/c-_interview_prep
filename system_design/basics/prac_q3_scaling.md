# 📊 Master Scale Reference Guide: Horizontal vs. Vertical Scaling

### Core System Scale Profile (The Baseline Data)
*   **Monthly Data Ingestion Footprint:** 20 TB / 30 Days ──> **667 GB/day** ──> **27.8 GB/hour**
*   **Document Upload Volume:** 5,000,000 / 30 Days ──> 166,666 docs/day ──> **2 docs/second (Average)**
*   **Audit Log Accumulation:** 100,000,000 / 30 Days ──> 3,333,333 lines/day ──> **38.5 lines/second (Average)**
*   **Peak Load Target (10x Ingestion Burst):** **20 docs/second** and **385 log lines/second**
*   **1-Year Cumulative Footprint:** **240 Terabytes** of raw text data and **1.2 Billion** audit log rows.

---

## 🟢 Part 1: Perfect Scenario for Horizontal Scaling ("Scale the Tier")

At a continuous ingest rate of 20TB per month, a distributed horizontal strategy handles the load optimally by isolating and scaling each tier independently using elastic, commodity hardware.

### Tier 1: C# Web API (Stateless Compute Layer)
*   **Infrastructure Design:** Deploy **3 small instances** (e.g., AWS `t3.medium` - 2 vCPU, 4GB RAM each) striped across 3 distinct Availability Zones (AZs) behind an Application Load Balancer (ALB).
*   **Why it Works:** Each small node handles its portion of the 640 Mbps peak network ingestion stream without blocking.
*   **Horizontal Trigger Rule:** Automatically spin up a new container instance whenever the average **CPU exceeds 75%** OR **Network Inbound reaches 100 Mbps** on any single node.

### Tier 2: Async Workers (Event-Driven Execution Layer)
*   **Infrastructure Design:** Set Kafka ingestion topics to exactly **128 partitions** to maximize our concurrency ceiling. Deploy an initial baseline pool of **16 optimized compute instances** (e.g., AWS `c6i.xlarge` - 4 vCPU, 8GB RAM).
*   **Why it Works:** Since dense document processing or OCR takes **5 seconds per file**, a single worker thread can only process 0.2 docs/sec. Spreading the load across a 128-partition consumer group allows over 100 concurrent threads to absorb a peak burst of 20 docs/sec smoothly.
*   **Horizontal Trigger Rule:** Spin up more container instances (up to the 128-partition cap) whenever **Kafka Consumer Lag exceeds 500 unread messages**.

### Tier 3: PostgreSQL Database (Stateful Metadata Layer)
*   **Infrastructure Design:** Initialize a **4-Node Sharded DB Cluster** (using an extension like Citus Data) partitioned natively by `Tenant_ID` (Client Law Firm ID). 
*   **Why it Works:** Writes and relational queries are isolated by tenant. Instead of one server handling 100 million logs, the write load is split mathematically across the 4 database nodes.
*   **Horizontal Trigger Rule:** Re-shard and add an extra database storage node to the cluster whenever an individual shard's data size crosses **500 Gigabytes** OR if sustained disk metrics cross **10,000 write IOPS**.

---

## 🔴 Part 2: The Hard Failures When Vertical Scaling the 20TB Scale

Attempting to scale the 20TB workload vertically by upgrading to a single, monolithic, ultra-high-spec machine introduces architectural barriers that cause absolute system failure.

### Tier 1 Failure: Thread Pool Exhaustion & OS Single Point of Failure (SPOF)
*   **The Blueprint Tried:** 1 massive web instance (e.g., AWS `c6i.8xlarge` - 32 vCPU, 64GB RAM).
*   **The Point of Failure:** If a sudden 10x morning traffic spike hits, the single operating system must manage a massive, unified web thread execution context. The application hits a hard ceiling bounded by network card stream capacity. If the OS patches, crashes, or suffers memory leaks, your **entire ingestion gateway is completely offline**.

### Tier 2 Failure: Compute Starvation & Context-Switching Degradation
*   **The Blueprint Tried:** 1 high-performance compute node (e.g., AWS `c6i.16xlarge` - 64 vCPU, 128GB RAM) reading from a single Kafka partition.
*   **The Point of Failure:** Because there is only 1 machine, you cannot utilize parallel Kafka worker groups. The node must spin up **100+ concurrent in-memory processing threads** to clear a 100,000 document backlog. The physical CPU registers become completely saturated by thread context-switching overhead, causing processing speeds to plummet and memory to exhaust.

### Tier 3 Failure: Storage Boundaries & RAM Index Starvation
*   **The Blueprint Tried:** 1 ultra-high-spec database server (e.g., AWS RDS `db.r6g.16xlarge` - 64 vCPU, 512GB RAM) with local provisioned disk space at 64,000 IOPS.
*   **The Point of Failure:** 
    1. **The RAM Wall:** To perform sub-second queries, PostgreSQL must fit its table indexes inside memory. Within 6 months, the B-Tree indexes for 600 million log lines surpass the 512GB RAM boundary. The **Cache Hit Ratio drops below 99%**, causing the database to thrash data continuously back and forth from slow disk storage.
    2. **The Backup/Restore Wall:** Within 1 year, the hot data accumulation hits **240 Terabytes**. Cloud providers cannot attach single block storage volumes of this size. Furthermore, routine database backups and replication catch-up windows become mathematically impossible to complete within standard operational hours.

---

## 🔵 Part 3: When the Numbers Are Perfect for Vertical Scaling

Vertical scaling is an optimal, highly stable choice when system volume is compact. It provides sub-millisecond query performance without the network complexity or data-split risks of a distributed sharded ring.

### Optimal System Scale Bounds
*   **Monthly Storage Growth:** **50 Gigabytes per month** (instead of 20TB).
*   **Document Upload Volume:** **~10,000 documents per month** (instead of 5 million).
*   **Audit Log Accumulation:** **~200,000 entries per month** (instead of 100 million).
*   **Per-Second Traffic:** Average of 1 upload every few minutes, with morning peak bursts capped at **3 to 5 documents per second**.
*   **3-Year Total Cumulative Footprint:** **~1.5 Terabytes max storage**.

### Perfect Vertical Tier Implementations

*   **Tier 1 (C# Web API):** **1 Single Cloud Instance** (e.g., AWS `c6i.xlarge` - 4 vCPU, 8GB RAM). At 5 uploads per second, CPU utilization stays at a highly comfortable **15% to 20%**. This gives the server 80% processing headroom to absorb peak morning traffic spikes without needing an elastic cluster or a load balancer configuration.
*   **Tier 2 (Async Workers):** **1 Compute Instance** (e.g., AWS `c6i.2xlarge` - 8 vCPU, 16GB RAM) reading from a 4-partition Kafka topic. The single worker application spawns an internal thread pool of **20 background threads**. Since 20 threads running 5-second tasks can process 4 documents per second, the 8 physical vCPUs handle the multi-threaded execution smoothly with zero context-switching delay.
*   **Tier 3 (PostgreSQL Database):** **1 Monolithic Instance** (e.g., AWS RDS `db.r6g.xlarge` - 4 vCPU, 32GB RAM). 
    *   *The Memory Fit:* At 50GB of database growth per year, your total operational metadata size after 3 years is 150GB. Relational B-Tree indexes comprise roughly 10% of table space (**~15 Gigabytes**). This means your **entire active index set fits completely inside memory with 17GB of safety overhead remaining** for OS caching and database operations.
    *   *The Result:* Your Cache Hit Ratio stays at a perfect **99.9%**, keeping queries sub-millisecond. You can apply simple **Local Range Table Partitioning by Month** on this single machine. Database backups take under 10 minutes, and replication streams seamlessly without network lag.
