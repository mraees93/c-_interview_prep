Scenario: Legal Document Ingestion & Compliance Analytics System1. 
System Requirements

Client Requests: Law firms and legal corporate clients log into a web application to upload large batches of legal contracts and court transcripts (PDF, DOCX, and scanned images) for automated compliance scanning and risk analysis.

contract service => 
    to upload large batches of legal contracts => use message queue/kafka with async workers to solve write-heavy problem
    the text contracts can then be stored in cassandra and court transcripts can be stored in aws blob storage

Third-Party Integration: The system must securely send the extracted text to external, specialized AI legal-compliance engines and government trademark/patent database APIs for validation. These external API calls are highly throttled, prone to intermittent timeouts, and take anywhere from 15 seconds to 3 minutes to return results.

use message queue/kafka with async workers to solve slow Third-Party api's, use dead letter queue for intermittent timeouts, 

**WRONG** implement polling every 10 secs in ui to check if results returned
**CORRECT** Use websockets to return results and eventually an email


Audit Logging & Immutability: Every document state change, user access, and third-party API payload must be permanently recorded. To comply with strict legal-tech data standards, these logs must be append-only, cryptographically verifiable to prove they have never been tampered with or deleted, and queryable with sub-second latency by legal auditors.

Use postgresql for acid, replication to handle possible db failures, use background async change data capture(CDC) **not dual writes** for syncing new updates into elasticsearch, proper indexing on tables, for index write speed failures i'll implement Table Partitioning (Range Partitioning by Date) name it by log info and month to speed up large table index bloating
caching for massive speed boost and frequently accessed data
elasticsearch for queries for sub-second latency
enforce a Zero-Trust network architecture on the different layers for cryptographically verifiable

Scale: 10,000 corporate legal teams uploading an average of 5 million total documents per month. This process generates approximately 100 million audit log entries and 20 Terabytes of raw data monthly.