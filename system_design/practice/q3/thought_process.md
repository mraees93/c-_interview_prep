Scenario: Legal Document Ingestion & Compliance Analytics System1. 
System Requirements

Client Requests: Law firms and legal corporate clients log into a web application to upload large batches of legal contracts and court transcripts (PDF, DOCX, and scanned images) for automated compliance scanning and risk analysis.

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
2. Contract Service (now holding the AWS S3 metadata pointer link) immediately drops that link into Kafka.
3. Kafka streams that link to your Workers.
4. Workers download the file from AWS S3, extract the text.
5. write it into Cassandra.

Whether a client uploads a scanned PDF or a text-only document, they all enter via the same pipeline. The Contract Service saves the raw file to S3 for compliance archiving, and drops a metadata pointer into Kafka. The background workers pull the pointer, read the file, process the text—whether that requires full OCR for images or just a stream read for text—and normalize the output before writing it to Cassandra.



Third-Party Integration: The system must securely send the extracted text to external, specialized AI legal-compliance engines and government trademark/patent database APIs for validation. These external API calls are highly throttled, prone to intermittent timeouts, and take anywhere from 15 seconds to 3 minutes to return results.

use message queue/kafka with async workers to solve slow Third-Party api's, use dead letter queue for intermittent timeouts, 

**WRONG** implement polling every 10 secs in ui to check if results returned
**CORRECT** Use websockets to return results and eventually an email


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

Scale: 10,000 corporate legal teams uploading an average of 5 million total documents per month. This process generates approximately 100 million audit log entries and 20 Terabytes of raw data monthly.