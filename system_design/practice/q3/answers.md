# Legal Document Ingestion & Compliance Analytics System Architecture

## 1. System Architecture Diagram

```text
[CLIENT TIER]              [EDGE TIER]               [INGESTION & DATA TIER]                [EVENT BACKBONE]            [ASYNC WORKER & AUDIT TIER]

+----------+              +-----------+             +--------------------+                 +----------------+          +--------------------+

|          | (1) HTTPS    |           | (2) Route   |  CONTRACT SERVICE  | (3a) Write Text |   CASSANDRA    |          |  3RD-PARTY SERVICE |
|          |------------->|    API    |------------>|   (MICROSERVICE)   |---------------->|   DATABASE     |    +---->|   (MICROSERVICE)   |
|          |              |  GATEWAY  |             +--------------------+                 +----------------+    |      +--------------------+
|   LAW    |              +-----------+                 |             ^                                          |        |                |
|   FIRM   |                    |                      (3b)          (3d) Cache Miss                             |       (6a) Async I/O   (7) Result Event
|  CLIENT  |              (2a) Session Check            v             |                                          |        v                v
|          |                    v                   +------+      +---------------+                              |      +-----------+    +---------------+
|          |              +-----------+             | AWS  |      |  REDIS CACHE  |                              |      | EXTI. GOV  |    |  NOTIFICATION |
|          |<-------------|   REDIS   |             |  S3  |      | (Doc Metadata)|                              |      |  BUREAUS  |    |    SERVICE    |
|          | (8) WebSockets/  (Auth)  |             +------+      +---------------+                              |      +-----------+    +---------------+
+----------+     Email    +-----------+                 |                                                            |                             |
                                                       (4) Publish Event                                             |                            (7a) Forward

                                                        |                                                            |                             v
                                                        v                                                            |                       +-----------+
                                                    +------------------------------------------------------------+   |                       |    API    |

                                                    | KAFKA DISTRIBUTED MESSAGE BROKER                           |   |                       |  GATEWAY  |
                                                    | - Topic: legal-document-ingestion -------------------------+---+                       +-----------+
                                                    | - Topic: audit-events-stream ------------------------------+--------------------+

                                                    | - Topic: third-party-dead-letter-queue (DLQ)               |                    |
                                                    +------------------------------------------------------------+                    |
                                                                                                                                     (9) Consume All
                                                                                                                                      |
                                                                                                                                      v
                                                                                                                             +--------------------+

                                                                                                                             |  AUDIT LOG SERVICE |
                                                                                                                             |   (MICROSERVICE)   |
                                                                                                                             +--------------------+
                                                                                                                                |
                                                                                                                               (10) Cryptographic Write
                                                                                                                                v
                                                                                                                             +--------------------+

                                                                                                                             | POSTGRESQL PRIMARY |
                                                                                                                             | (Date Partitioned) |
                                                                                                                             +--------------------+
                                                                                                                                |
                                                                                                                               (11) WAL Capture
                                                                                                                                v
                                                                                                                             +--------------------+

                                                                                                                             |   DEBEZIUM (CDC)   |
                                                                                                                             +--------------------+
                                                                                                                                |
                                                                                                                               (12) Stream Index
                                                                                                                                v
                                                                                                                             +--------------------+

                                                                                                                             |   ELASTICSEARCH    |
                                                                                                                             +--------------------+
                                                                                                                                ^
                                                                                                                               (13) Sub-Second Read
                                                                                                                                |
                                                                                                                             +--------------------+

                                                                                                                             |  COMPLIANCE AUDIT  |
                                                                                                                             |    DASHBOARD       |
                                                                                                                             +--------------------+
```

### Diagram Legend & Flow Notes

#### Write Path (Document Processing Pipeline)
*   **(1) to (2):** The Client initiates a batch upload via HTTPS. The API Gateway queries **Redis (2a)** to authenticate active user sessions and enforce rate limiting before routing the request to the decoupled **Contract Service**.
*   **(3a) & (3b):** The isolated **Contract Service** splits the payload, uploading the heavy raw binary data directly to **AWS S3** and writing the extracted text structure into **Cassandra**.
*   **(3c) & (3d):** Metadata and transitional tracking information are populated directly into **Redis** using a **Cache-Aside** strategy to offload expensive database reads.
*   **(4):** The Contract Service instantly publishes an `Inbound_Document` token into the **Kafka Broker** using the Document UUID as the partition key. It safely terminates its own transaction without blocking on external systems.
*   **(5):** The **Third-Party Service** acts as an independent event-driven consumer, reading messages instantly via dedicated consumer poll threads.
*   **(6a):** Handover occurs to a concurrent execution worker loop that securely addresses the slow external APIs (15s–3min processing frames) of external government and credit bureaus.

#### Read Path & User Notification
*   **(7) to (8):** Upon receiving third-party response vectors, the **Third-Party Service** fires an execution event down the broker pipeline. The **Notification Service** intercepts this data and targets the active **API Gateway** instance, updating the user immediately over an active **WebSocket** socket or dropping a confirmation **Email**.

#### Write & Read Paths for Auditing & Compliance
*   **(9):** Separated entirely from the data routing mechanics, the **Audit Log Service** consumes all raw infrastructure transitions off the Kafka broker logs.
*   **(10):** The service streams logs into **PostgreSQL**. Tables utilize **Range Partitioning by Date** (monthly rotation) to avoid global index bloat.
*   *Cryptographic Implementation Note:* Individual log nodes generate an application-level linked **Cryptographic Hash Chain** (`Hash(Current Row Data + Prior Row Hash)`). Any low-level database modification fractures the system sign-offs, alerting compliance frameworks.
*   **(11) to (12):** **Debezium Change Data Capture (CDC)** tails the Postgres Write-Ahead Log (WAL) engine completely out-of-process, mirroring structures down to **Elasticsearch** without risking dual-write split-brain synchronization errors.
*   **(13):** Auditor compliance panels pull records directly out of the fast **Elasticsearch** tier for real-time validation.


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
