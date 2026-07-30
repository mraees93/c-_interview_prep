Scenario: Legal Document Ingestion & Compliance Analytics System1. 
System Requirements

1. Client Requests: Law firms and legal corporate clients log into a web application to upload large batches of legal contracts and court transcripts (PDF, DOCX, and scanned images) for automated compliance scanning and risk analysis.

Thoughts:
contract service => 
    to upload large batches of legal contracts => use message queue/kafka with async workers to solve write-heavy problem
    the text contracts can then be stored in cassandra and court transcripts can be stored in aws blob storage


### Correct Drawing Ingestion Path Sequence

* **1. DNS & Load Balancing:** The client application resolves our entry point domain via DNS Geo-Routing. This balances traffic directly across multiple high-availability Application Load Balancers (ALBs).
* **2. Edge Validation:** The ALB handles ingress traffic routing and forwards requests to our API Gateway tier, which actively manages central authentication, rate limiting, and mTLS termination.
* **3. File Ingestion & Object Storage:** The API Gateway forwards the raw multipart batch request to the `Contract Service`. To prevent Kafka payload bloat, this service writes the heavy raw binary files (PDFs, DOCX, scans) straight to an **AWS S3** bucket first.
* **4. Lightweight Event Decoupling:** Once the payload safely commits to S3, the `Contract Service` publishes a small, lightweight tokenized event to **Kafka** containing only metadata and the S3 file pointer (e.g., `{"document_id": "123", "s3_url": "s3://..."}`).
* **5. Async Worker Extraction:** Independent background consumer workers pull this lightweight message off their designated Kafka partitions, download the raw document from **AWS S3**, run the compliance extraction logic, and write the parsed text securely into **Cassandra**.

Raw batch upload on my drawing:
1. Contract Service sends the heavy raw file into AWS S3.
2. Producer Execution, Contract Service publishes the request token to the Kafka Requests Topic. It attaches a key-value attribute inside the Kafka Message Headers 
    (e.g., `target-db: postgres` or `target-db: cassandra`) to signal the downstream data requirements. Contract Service (now holding the AWS S3 metadata pointer link) immediately drops that link into Kafka. 
3. Independent background consumer workers pull this lightweight message off their designated Kafka partitions.
4. Workers download the file from AWS S3, extract the text.
5. write it into Cassandra.
**if rabbitMQ: Independent workers asynchronously pull messages out of the RabbitMQ queue**
Whether a client uploads a scanned PDF or a text-only document, they all enter via the same pipeline. The Contract Service saves the raw file to S3 for compliance archiving, and drops a metadata pointer into Kafka. The background workers pull the pointer, read the file, process the text—whether that requires full OCR for images or just a stream read(tiny JSON message string) for text—and normalize the output before writing it to Cassandra.



2. Third-Party Integration: The system must securely send the extracted text to external, specialized AI legal-compliance engines and government trademark/patent database APIs for validation. These external API calls are highly throttled, prone to intermittent timeouts, and take anywhere from 15 seconds to 3 minutes to return results.

use message queue/kafka with async workers to solve slow Third-Party api's, use dead letter queue for intermittent timeouts, 

**WRONG** implement polling every 10 secs in ui to check if results returned
**CORRECT** Use websockets to return results and eventually an email

for my drawing:
[Third-Party Service] ➔ [Kafka: Requests] ➔ [Workers] ➔ [Kafka: Completions] ➔ [Notification Service] ➔ [Client Browser]

Step 6: The Third-Party Service drops a verification token into the Kafka Requests Topic.
Step 7: Kafka buffers the token securely across its partitions.
Step 8: Workers pull the token from the requests topic and execute the slow 3-minute external APIs out-of-band.
Step 9: Once done, the Worker publishes a completion token onto the Kafka Completions Topic.
Step 10: The decoupled Notification Service consumes that completion token from Kafka.
Step 11: The Notification Service pushes the data down the open WebSocket straight to the Client Browser.



Audit Logging & Immutability: Every document state change, user access, and third-party API payload must be permanently recorded. To comply with strict legal-tech data standards, these logs must be append-only, cryptographically verifiable to prove they have never been tampered with or deleted, and queryable with sub-second latency by legal auditors.

Use postgresql for acid, 
replication for high availability, 
db failover to handle possible db failures, 
proper indexing on tables, 
**right idea**for index write speed failures i'll implement Table Partitioning (Range Partitioning by Date) name it by log info and month to speed up large table index bloating.
**better**For index write speed failures, I'll implement Table Partitioning—specifically Range Partitioning by log info and monthly intervals. This keeps the active write index small, directly mitigating (make less severe) large-table index bloat.
caching for massive speed boost and frequently accessed data
elasticsearch for queries for sub-second latency
use background async change data capture(CDC) **not dual writes** for syncing new updates into elasticsearch, 
enforce a Zero-Trust network architecture on the different layers for cryptographically verifiable



**can i configure some kafka workers in the pool to either send to cassandra or postgresql? Refer to steps 6 and 9A for this specific scenario**

**Metadata/Header-based Routing pipeline (Cleanest solution):**

*   **Step 6 (Producer Execution):** The Third-Party Service publishes the request token to the Kafka Requests Topic. It attaches a key-value attribute inside the Kafka Message Headers (e.g., `target-db: postgres` or `target-db: cassandra`) to signal the downstream data requirements.

*   **Step 7 (Secure Buffering):** Kafka buffers the request token and its routing headers securely across its partitions.
*   **Step 8 (Out-of-Band Execution):** Workers pull the token from the requests topic, read the headers, and execute the slow 3-minute external APIs out-of-band.

*   **Step 9A (Dynamic Ingestion Router):** Immediately after the API returns the payload, the worker checks that original header attribute. It instantly hands the third-party API payload directly to the correct database connection pool (**PostgreSQL**) as explicitly requested by the producer header in **step 6**.

**This entire data synchronization flow (from Step 10 all the way to Step 13) happens completely in the background (asynchronously).**
10. Replication:
    PostgreSQL: The primary instance records the transaction to its local **Write-Ahead Log (WAL)** and automatically streams these binary changes to the read replicas via background processes.
    
    Trade-off: This native approach guarantees data consistency and prevents application bloat, but introduces a tiny delay called **Replication Lag** before the data becomes readable on the replica instances.

11. The Read Replicas stream those replication logs **(Write-Ahead Logs, WAL)** directly into the CDC Engine (Debezium) out-of-band.
12. The CDC Engine converts those database row mutations into events and publishes them onto a dedicated Kafka Change-Log Topic.
13. An Elasticsearch Sink consumes those events from Kafka and indexes the data into Elasticsearch for sub-second searching.

Scale: 10,000 corporate legal teams uploading an average of 5 million total documents per month. This process generates approximately 100 million audit log entries and 20 Terabytes of raw data monthly.