System Requirements:

Client Requests: Financial institutions and international law firms call our high-speed REST API to run real-time compliance checks on entities (individuals or corporations). They need to verify if an entity has been flagged in global court rulings, asset-forfeiture records, or government sanction lists.

DNS => multiple load balancers for high availability => api gateway => compliance check microservice =>
implement redis cache for high speed reads and frequently accessed entities and their "Match/No Match" responses 
**BETTER WAY** implement a Redis cache layer using a Cache-Aside strategy to capture high-speed reads and offload frequently accessed entity status lookups.
postgresql for acid on all entitites
proper indexing, for large table index bloat i'll implement range table partitioning specifically by month
horizontal scaling with sharding, for hot shards problem i'll implement composite shard keys based on global court rulings, asset-forfeiture records, government sanction lists + the year for high availability

Challenge 1: The PostgreSQL Cross-Shard Unique Constraint Failure:

The Issue: PostgreSQL cannot natively enforce a unique index constraint across completely separate physical sharded database nodes. If an entity name is written to Node A, Node B cannot see it in real-time, allowing duplicate entity records to corrupt your system.

The Answer: Enforce unique entity constraints at the application layer before the data hits the database. Generate a unique Entity UUID using a Deterministic Hashing Engine (like SHA-256) on the entity's data. Use a high-speed Redis Bloom Filter at your ingestion entry point to check if that unique hash already exists in the system with negligible memory overhead.


Challenge 2: The Relational vs. Distributed NoSQL Trade-off

The Issue: Forcing PostgreSQL to shard horizontally across multiple servers requires complex custom routing scripts, manual partitioning management, and high operational overhead for what is essentially a high-speed key-value lookup.

The Answer: Defend your choice of PostgreSQL by emphasizing the business need for complex relational data querying (e.g., linking a flagged entity to specific court case IDs, assets, and regulatory articles). State that if the query patterns drop to a flat key-value lookup, you would migrate the core data tier to a native distributed NoSQL store like Cassandra or AWS DynamoDB to handle partitioning and clustering natively




Third-Party Integration: The system must securely ingest real-time updates from 15 external regulatory frameworks (e.g., INTERPOL, UN Security Council, South African Reserve Bank, UK OFSI). These third parties publish changes via an unpredictable mix of daily XML file uploads, webhook streams, and slow REST endpoints that frequently experience network timeouts.



Performance & Availability: API checks from banking clients must return a definitive "Match/No Match" response within less than 150 milliseconds to avoid slowing down credit card transactions or wire transfers.



Scale: The screening database must maintain records for 200 million distinct entities globally. Banking clients generate up to 8,000 read requests per second (RPS) during global market hours.