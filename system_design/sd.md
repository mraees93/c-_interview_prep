# LexisNexis Scalable Search Architecture Diagram

```text
                                      [ USER / LEGAL PRO ]
                                               │
                                               ▼
┌──────────────────────────────────────┬──────────────────────────────────────┬──────────────────────────────────────┐
│ BUCKET 1: TO GET THEM IN             │ BUCKET 2: TO MAKE IT FAST            │ BUCKET 3: TO SCALE                   │
│ (Access Control & Security)          │ (The Read Path)                      │ (The Write Path)                     │
├──────────────────────────────────────┼──────────────────────────────────────┼──────────────────────────────────────┤
│                                      │                                      │                                      │
│  [1. DNS & WAF]                      │  [4. CDN Edge Cache]                 │                                      │  ◄── LAYER: EDGE
│        │                             │        ▲                             │                                      │      (External)
│        ▼                             │        │ (Edge Hit)                  │                                      │
│  [2. Load Balancer]                  │        │                             │                                      │
│        │                             │        │                             │                                      │
│        ▼                             │        │                             │                                      │
│  [3. API Gateway & Rate Limiter] ────┼───(Search Request)───────────────────┼──► [8. Upload Service]               │  ◄── LAYER: APPLICATION
│                                      │   └─► [5. Search Service (.NET Core)]│          │                           │      (Microservices)
│                                      │             │                        │          ▼                           │
│                                      │             ▼                        │    [9. Message Queue]                │
│                                      │       [6. Redis Cache]               │          │                           │
│                                      │             │                        │          ▼                           │
│                                      │             ▼                        │    [10. Indexing Worker]             │  ◄── LAYER: DATA
│                                      │       [7. Elasticsearch] ◄───────────┼────(Update Index)                    │      (Cache/Index)
│                                      │                                      │                                      │
│                                      │                                      │          │                           │
│                                      │                                      │          ▼                           │
│                                      │                                      │    [11. DB & Blob Storage]           │  ◄── LAYER: INFRASTRUCTURE
│                                      │                                      │        (SQL Server / Azure)          │      (Master Storage)
└──────────────────────────────────────┴──────────────────────────────────────┴──────────────────────────────────────┘
```

## Architectural Component Definitions

### Bucket 1: To Get Them In (Access Control)
* **1. DNS & WAF:** Resolves the routing and inspects traffic at the edge to block malicious payloads (e.g., SQL injections, XSS) before it reaches the backend network.
* **2. Load Balancer:** Evenly distributes raw Layer 7 HTTP/HTTPS traffic to prevent server bottlenecks.
* **3. API Gateway:** Handles common infrastructure tasks like JWT token authorization, request routing, and microservice aggregation.

### Bucket 2: To Make It Fast (The Read Path)
* **4. CDN Edge Cache:** Serves static visual components (Angular UI framework bundle) and frequently requested static legal PDFs straight from edge points.
* **5. Search Service:** High-throughput backend service tailored specifically to handle structured user queries.
* **6. Redis Cache:** Stores common search outputs in memory, shielding the main indexing cluster from processing identical queries repeatedly.
* **7. Elasticsearch:** High-performance engine built explicitly for document scanning. It utilizes an inverted index structure to deliver search hits across millions of logs instantly.

### Bucket 3: To Scale (The Write Path)
* **8. Upload Service:** Validates incoming file schemas and returns a `202 Accepted` receipt to the client immediately to prevent UI lag.
* **9. Message Queue:** Asynchronous messaging line that absorbs massive transactional ingestion bursts without impacting consumer search operations.
* **10. Indexing Worker:** Worker engines that consume queue payloads, run text/OCR extraction pipelines, and structural formatting.
* **11. DB & Blob Storage:** Relational database storage holding highly available master schemas alongside immutable Azure Cloud/S3 binary storage buckets for primary document files.
