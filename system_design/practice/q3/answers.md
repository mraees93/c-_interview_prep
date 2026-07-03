# Legal Document Ingestion & Compliance Analytics System Architecture

## 1. System Architecture Diagram

```text
+----------+      (1) HTTPS Upload     +-------------+      (2) Route Request     +------------------+

|          |-------------------------->|             |--------------------------->|                  |
|   LAW    |                           |     API     |                            |     CONTRACT     |
|   FIRM   |                           |   GATEWAY   |                            |     SERVICE      |
|  CLIENT  |                           |  / PROXY    |                            |                  |
|          |<--------------------------|             |<---------------------------|                  |
+----------+      (8) WebSockets/Email +-------------+      (7) Alert Signal      +------------------+

                                                                                    |              |
                                                                                   (3a)           (3b)

                                                                                    |              |
                                                                                    v              v
                                                                              +-----------+  +-----------+

                                                                              |  AWS S3   |  | CASSANDRA |
                                                                              | (Raw PDF) |  |   (Text)  |
                                                                              +-----------+  +-----------+
                                                                                    |
                                                                                   (4) Publish Inbound Event
                                                                                    |
                                                                                    v
                                                                      +----------------------------------+

                                                                      | KAFKA DISTRIBUTED MESSAGE BROKER |
                                                                      | - legal-document-ingestion       |
                                                                      | - audit-events-stream            |
                                                                      | - third-party-dlq                |
                                                                      +----------------------------------+

                                                                          |                          |
                                                                         (5) Consume                (9) Consume All

                                                                          |                          |
                                                                          v                          v
                                                              +------------------+        +------------------+

                                                              |   THIRD-PARTY    |        |    AUDIT LOG     |
                                                              |  SERVICE POOL    |        |     SERVICE      |
                                                              +------------------+        +------------------+

                                                                  |          |                       |
                                                                 (6a)       (6b)                    (10) Write Row
                                                                  v          v                       v
                                                              +-------+  +-------+        +------------------+

                                                              | Gov   |  | Credit|        |    POSTGRESQL    |
                                                              | Bureau|  | Bureau|        | (Date Partition) |
                                                              +-------+  +-------+        +------------------+
                                                                                                     |
                                                                                                    (11) WAL Capture
                                                                                                     |
                                                                                                     v
                                                                                          +------------------+

                                                                                          |  DEBEZIUM (CDC)  |
                                                                                          +------------------+
                                                                                                     |
                                                                                                    (12) Stream Index
                                                                                                     |
                                                                                                     v
                                                                                          +------------------+

                                                                                          |  ELASTICSEARCH   |
                                                                                          +------------------+
                                                                                                     ^
                                                                                                     |
                                                                                          (13) Sub-Second Query
                                                                                                     |
                                                                                          +------------------+

                                                                                          | COMPLIANCE AUDIT |
                                                                                          |   DASHBOARD      |
                                                                                          +------------------+
```

### Diagram Legend & Flow Notes

#### Write Path (Document Processing Pipeline)
*   **(1) to (2):** Client uploads large batches of legal documents securely via HTTPS. The API Gateway routes traffic to the **Contract Service**.
*   **(3a) & (3b):** The **Contract Service** strips raw binary files to **AWS S3** and saves structured text content to **Cassandra** for distributed access.
*   **(4):** The Contract Service instantly fires an `Inbound_Document` event to the **Kafka Broker** using the Document UUID as the partition key. It does *not* wait for third-party APIs.
*   **(5):** The **Third-Party Service** utilizes a dedicated polling thread to pull messages swiftly from Kafka without stalling partitions.
*   **(6a) & (6b):** Handover occurs to a highly concurrent **Worker Pool Execution Thread** that processes long-running (15s–3min) external calls securely to government and credit bureaus. If timeouts continuously fail, messages route to the **DLQ**.

#### Read Path & User Notification
*   **(7) to (8):** Once the worker receives a payload, it triggers the **Notification Service**, sending real-time job status updates via a persistent **WebSocket** connection or triggering a transactional **Email** confirmation.

#### Write & Read Paths for Auditing & Compliance
*   **(9):** The **Audit Log Service** listens as an independent consumer to all events across Kafka.
*   **(10):** Logs stream to **PostgreSQL**. Tables use **Range Partitioning by Date** (monthly cycles) to maintain small index footprints and fast ingestion rates.
*   *Security Implementation Note:* Every row written encapsulates an application-level **Cryptographic Hash Chain** (`Hash(Current Data + Prior Row Hash)`). 
*   **(11) to (12):** **Debezium Change Data Capture (CDC)** reads the Postgres Write-Ahead Log (WAL) asynchronously and replicates logs directly into **Elasticsearch**. This bypasses dual-write inconsistency and prevents transactional degradation.
*   **(13):** Compliance Auditors read logs through a dedicated interface interacting with **Elasticsearch**, ensuring sub-second query latency over hundreds of millions of past entries.

***

## 2. Interview Simulation Log: Questions, Feedback, and Correct Answers

### Round 1: Asynchronous Processing & Worker Bottlenecks

*   **Question Asked (Interviewer 1 - Backend & Scaling Lead):** 
    "If a burst of law firms uploads 100,000 documents at 09:00 AM, our workers will get blocked waiting on those slow external APIs. How will you configure your Kafka topics, partitioning, or worker pool to ensure that a backup in one slow third-party API doesn't completely freeze the ingestion pipeline for everyone else? Also, tell me why you chose polling instead of WebSockets or Server-Sent Events (SSE) for the UI at this scale."
*   **Your Feedback / Response:** 
    *   *Worker Pool:* Implement Decoupled Worker Pools. Kafka workers use a dedicated polling thread to fetch messages swiftly and hand them off to a large concurrent Execution Thread Pool.
    *   *Kafka Partitioning:* Over-partition topics into 32 to 64 segments using a unique identifier (Document UUID) as a message key to distribute load evenly.
    *   *UI Strategy:* Acknowledged that a 10-second polling interval creates unnecessary overhead. Pivoted to WebSockets for instant updates and added an email fallback notification.
*   **Correct / Optimal System Design Answer:** 
    Your answer was correct and optimal. Isolating the Kafka consumer thread loop from the I/O-bound network execution context protects partitions from starving. Over-partitioning by a high-cardinality key avoids hot partitions, and replacing periodic HTTP polling with push mechanisms (WebSockets/SSE) decreases web tier request bloat at scale.

### Round 2: Data Replication & The Dual-Write Problem

*   **Question Asked (Interviewer 2 - Data & Analytics Architect):** 
    "At a scale of 100 million new audit log entries a month, data integrity is critical. How exactly does a log entry get into both places [PostgreSQL and Elasticsearch]? If you use dual-writes from the application layer, one write might succeed while the other fails due to a network glitch, causing data drift. How will you design the pipeline to guarantee that every single log entry written to Postgres is reliably reflected in Elasticsearch without slowing down the core ingestion system?"
*   **Your Feedback / Response:** 
    After data writes to PostgreSQL and begins replicating to standard database replicas, a background Change Data Capture (CDC) engine (specifically Debezium) automatically monitors transaction logs and pushes asynchronous stream updates out to Elasticsearch.
*   **Correct / Optimal System Design Answer:** 
    Your answer was 100% correct. Transaction log mining/CDC via utilities like Debezium is the standard pattern for solving the dual-write problem. It ensures eventual consistency by relying on the database's atomic Append-Only Write-Ahead Log (WAL) and completely untangles the core application from secondary-index operational failures.

### Round 3: Data Tampering vs. Infrastructure Security

*   **Question Asked (Interviewer 3 - Security & Compliance Officer):** 
    "The client explicitly stated that the audit logs must be *cryptographically verifiable* to prove they have never been altered or deleted. If a rogue system administrator with root access or a hacker compromises our PostgreSQL database, they could theoretically alter records directly in the storage layer. How will you use cryptography (such as hashing, block-chaining, or Merkle trees) within your database or ingestion design to prove to an external legal auditor that an audit log entry from six months ago is 100% untampered with?"
*   **Your Feedback / Response:** 
    *   *Initial Response:* Proposed Mutual TLS (mTLS) for in-transit communication and strict VPC network isolation / Role-Based Access Control (RBAC) to enforce least privilege access.
    *   *Pivoted Response (After pushback on infrastructure vs data-at-rest tampering):* Opted for an **Immutable Audit Log** architecture utilizing an **Application-Level Hashing Chain of Trust**, ensuring that any unauthorized database alteration breaks the hash chain so that an auditor instantly spots modifications.
*   **Correct / Optimal System Design Answer:** 
    The optimal path requires writing data with cryptographic structural integrity. By implementing an application-managed **cryptographic hash chain** (or adopting a hardware/cloud ledger storage framework featuring a Merkle Tree verification system like AWS QLDB), the data proves its own state. If an individual modifies historical database values using root privileges, the linked cryptographic signatures fail validation sequentially, rendering the breach immediately obvious to compliance tooling.
