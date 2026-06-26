# LexisNexis Interview Preparation Master File
**Target Company:** LexisNexis Cape Town  
**Role Level:** Intermediate Software Engineer  
**Case Study 2:** Corporate Compliance & Audit Logging System

---

## 1. System Requirements

*   **Client Requests:** Corporate clients log into a web dashboard and submit high-volume background screening requests.
*   **Third-Party Integration:** The system must connect securely to government and credit bureau APIs. These external calls are highly unpredictable and take anywhere from **30 seconds to 2 minutes** to respond.
*   **Audit Logging:** Every single system event must be permanently logged. These logs are strictly write-only, must never be altered or deleted by anyone, and must be easily searchable by compliance officers.
*   **Scale:** 5,000 corporate clients generating up to 2 million background checks per month, resulting in **50 million new audit log entries every single month**.

---

## 2. System Architecture Diagram (Step-by-Step Flow)

### Legend:
*   `●━━►` = **Core Background Check Flow** (Write-heavy LSM-tree operations)
*   `◌┈►` = **Asynchronous API & Audit Logging Flow** (Decoupled background tasks)

```text
                                                              ┌─────────────────┐
                                                       [W2b]  │Cassandra AP Core│
                                                       ◌━━━━━►│  (LSM-Tree DB)  │
                                                       │      └─────────────────┘
┌─────────┐   ┌───────────┐    ┌─────────┐    ┌────────┴────────┐
│Corporate│●─►│   Load    │●──►│   API   │●──►│   Ingestion     │
│ Clients │   │ Balancer  │    │ Gateway │    │    Service      │
└─────────┘   └───────────┘    └─────────┘    └────────┬────────┘
   [R1]                                                │ 
   [W1]                                          [W2a] ◌ (Async Request Payload)
                                                       ▼
                                              ┌─────────────────┐
                                              │  RabbitMQ Bus   │
                                              └────────┬────────┘
                                                       │ [W3] (Queue Consumer)
                                                       ▼
                                              ┌─────────────────┐
                                              │  Async Workers  │
                                              └────┬────────┬───┘
                                     [W4a] (Write) │        │ [W4b] (External API Call)
                                                   ▼        ▼
┌──────────────┐     ┌──────────────┐         ┌────┴────┐ ┌─────────────────┐
│Elasticsearch │◄━━━━┨ CDC Engine   │◄────────┨Postgres │ │Slow External API│
│Search Cluster│ [W6]│ (Debezium)   │ [W5]    │Primary  │ │ (Govt / Credit) │
└──────────────┘     └──────────────┘ (Replica│  (ACID) │ └─────────────────┘
                                        Logs) └─────────┘
```

### Flow Walkthrough Comments:

#### Core Submission Flow (Steps R1 & W1 to W2b)
*   **[R1 / W1]**: A corporate user submits a batch of employee background screening checks from their dashboard.
*   **[W2a]**: To prevent long-running downstream tasks from locking the web instance, the Ingestion Service immediately offloads the request body payload onto **RabbitMQ**.
*   **[W2b]**: Simultaneously, the active transaction states are persisted directly inside the write-optimized **Cassandra AP Core Cluster**.

#### Asynchronous Worker & Audit Logging Flow (Steps W3 to W6)
*   **[W3]**: A pool of horizontally scaled **Async Workers** consume the request jobs out of RabbitMQ asynchronously.
*   **[W4a / W4b]**: The worker triggers the intensive **External API Calls** to verify details. As events happen, the worker writes immutable records directly into the **PostgreSQL Primary** database to establish the ACID-compliant audit trail.
*   **[W5 / W6]**: To keep things performant, an active **Change Data Capture (CDC)** engine tails the replica transaction logs and syncs new entries straight to an **Elasticsearch Search Cluster** where compliance auditors can run reports.

---

## 3. The 7 Interview Questions, Answers, & Explanations

### Question 1: Cassandra & LSM-Tree Read Penalties
*   **Question:** Because Cassandra uses an LSM-Tree storage structure, it flushes data to disk as multiple separate immutable SSTable files over time. When a user requests a read to view a check's current status, how do you prevent Cassandra from suffering a severe "read penalty" by searching through multiple files?
*   **Your Answer:** "we can use a Partition Key Index, this would make the lookup lightning-fast... Cassandra runs a continuous background process called Compaction. It processes a new sstable and the Partition Key Index would be used on the new table"
*   **Full Explanation Answer:** Spot on. The Partition Key Index allows Cassandra to navigate directly to the target node and partition, avoiding cluster-wide scans. To clean up file fragmentation inside that partition, Cassandra runs an automated background process called **Compaction**. Compaction reads multiple old SSTables, merges the latest updates, drops deleted rows, and writes a single clean consolidated file to disk. To optimize this even further, Cassandra leverages **Bloom Filters** in memory to check if a partition key exists within a file *before* performing an expensive disk read.

### Question 2: Data Consistency Across Storage Boundaries
*   **Question:** You chose PostgreSQL for the audit log because of strict ACID compliance, but Cassandra is an eventual consistency (AP) platform. If a status changes in Cassandra but fails to log successfully in your PostgreSQL audit trail, how do you prevent your system from entering a mismatched, out-of-sync state?
*   **Your Answer:** *(Handled as part of the architecture choices evaluation)*
*   **Full Explanation Answer:** To prevent distributed consistency mismatches, you must not use dual-writing from the application tier. Instead, use an **Event-Driven Architecture with Transactional Outbox Patterns**. When a background check state updates inside Cassandra, an atomic event message is saved locally. A reliable log publisher pattern extracts this message and pushes it through the messaging infrastructure (RabbitMQ) directly into the PostgreSQL audit consumer queue, guaranteeing eventual consistency across boundaries.

### Question 3: Absorbing Database Write Spikes
*   **Question:** With a massive stream of 50 million incoming audit records constantly trying to hit your relational database, how will you prevent the write pipeline from bottlenecking and slowing down the downstream Elasticsearch sync process?
*   **Your Answer:** "i would include rabbitmq with a worker... and use non-clustered indexes"
*   **Full Explanation Answer:** This is exactly right. Placing RabbitMQ with an asynchronous worker pool in front of the database acts as a durable throttle. If a massive burst of background checks completes simultaneously, RabbitMQ safely queues the audit log payloads, allowing your database workers to steadily consume and insert records at a stable, sustainable pace. Keeping indexing non-clustered allows physical table rows to be appended sequentially on disk.

### Question 4: Relational Table Index Bloat Management
*   **Question:** PostgreSQL uses a B-Tree indexing structure. Inserting 50 million rows a month will bloat the table index tree structure, memory usage will spike, and write speeds will eventually fail. How would you optimize the physical database tables to prevent this index breakdown?
*   **Your Answer:** "normalization to split the big tables by the dates"
*   **Full Explanation Answer:** Splitting the large datasets by date is the right intuition, but the exact technical database implementation is called **Table Partitioning** (specifically, **Range Partitioning by Date**). Instead of forcing PostgreSQL to maintain one giant B-Tree index across hundreds of millions of records, the table is divided into clean, time-bound slices (e.g., a table partition per month, like `audit_logs_2026_june`). New logs are appended only to the active month's light index, preserving peak write performance.

### Question 5: Maintaining Elasticsearch Search Synchronization
*   **Question:** After writing the audit logs directly into PostgreSQL, how do you plan to sync that new data over to Elasticsearch without degrading performance or losing consistency during server crashes?
*   **Your Answer:** "after writing the audit logs directly into PostgreSQL, the data should sync into the replica db's. Afterwards a background Change Data Capture (CDC) engine reads the new postgresql transaction records and updates the elasticsearch indexes so its searchable"
*   **Full Explanation Answer:** This is a masterful, senior-level response. By pointing your **Change Data Capture (CDC)** tool (such as Debezium) directly at your database read replicas, you ensure that the high-throughput synchronization process extracts data asynchronously from transaction logs, placing zero overhead on your primary write node. Because CDC tracks physical database logs, if the application or search cluster goes down, it can resume reading from its exact log position upon recovery, guaranteeing no dropped data.

### Question 6: Poison Pill Error Handling in Queues
*   **Question:** Suppose a corporate client submits bad data that causes a third-party API to return a `400 Bad Request` every time it's retried. If your worker rejects the message and places it back in RabbitMQ, it creates an infinite, resource-draining crash loop. How do you isolate this?
*   **Your Answer:** "i will implement a dead letter queue"
*   **Full Explanation Answer:** Excellent. Implementing a **Dead Letter Queue (DLQ)** handles this issue gracefully. The worker code catches explicit unrecoverable application errors or tracks retry counts. Once a message exceeds its retry threshold, RabbitMQ routes it to a designated DLQ. This isolates corrupt data instantly, freeing up your active worker pool to process legitimate requests while alerting developers to inspect the bad payloads.

### Question 7: Zero-Trust Security Implementation
*   **Question:** If an attacker compromises a single container and gains access to the internal virtual network segment to sniff moving packets, what security strategies will you implement across your database and queue layers to prevent data theft?
*   **Your Answer:** "i will enforce a Zero-Trust network architecture on the different layers... (Unsure on the technical implementation steps)"
*   **Full Explanation Answer:** Implementing a Zero-Trust Architecture means assuming the internal network is always hostile. To make this operational across your layers:
    1.  **Data-In-Transit Encryption**: Enforce strict **mTLS (Mutual TLS)** for all internal service communication. Every connection from your API gateway to RabbitMQ, or from workers to PostgreSQL and Cassandra, must be encrypted over the wire, rendering network sniffing completely useless.
    2.  **Identity & Access Management (IAM)**: Eliminate hardcoded configuration passwords. Utilize centralized secret engines (like AWS Secrets Manager, Azure Key Vault, or HashiCorp Vault) paired with short-lived **IAM database authentication tokens**. Application containers use their managed runtime identities to securely request ephemeral tokens that automatically expire, neutralizing the threat of stolen configuration credentials.

