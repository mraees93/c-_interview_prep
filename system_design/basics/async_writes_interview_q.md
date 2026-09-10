# ✉️ Top Technical Interview Questions: RabbitMQ & Distributed Workers
*LexisNexis Architectural Panel Defense Module*

---

# 🎖️ LexisNexis Messaging Strategy Cheat Sheet

When answering RabbitMQ design questions for the Cape Town panel, anchor every single scenario into three pillars of stability:

1. **Never Assume Network Success:** Always specify `autoAck = false` on consumers. The message only dies inside the broker when the database transaction officially commits to disk.
2. **Defend the Cluster Balance:** Enforce `BasicQos(prefetchCount: 1)` across all competing consumer nodes to ensure Docker container replicas scale horizontally and load-balance perfectly.
3. **Isolate the Blast Radius:** Never let an execution error spiral into an infinite retry loop. Leverage Dead Letter Exchanges (DLX) to pull faulty payloads out-of-band instantly, keeping production traffic unblocked.

---

### ❓ Q1: What is the difference between RabbitMQ and Apache Kafka? When would you choose one over the other?

*   **The Conceptual Difference:** **RabbitMQ is a smart message broker with dumb consumers.** It relies on routing rules (Exchanges) to deliberately push messages to specific queues and deletes messages the moment they are safely processed and acknowledged. **Apache Kafka is a dumb broker with smart consumers.** It is a continuous, high-performance distributed commit log appended to a disk buffer. It does not push messages; consumers use their own internal marker indices (Offsets) to pull data out of a stream at their own pace, and messages remain on disk long after they are read.
*   **When to choose RabbitMQ:** Choose RabbitMQ when your architecture requires **complex routing matrix rules** (e.g., routing legal case updates based on regional categories using custom wildcard routing keys), explicit payload delivery guarantees, or instant consumer-to-consumer point tasks.
*   **When to choose Kafka:** Choose Kafka for **extreme high-throughput event streaming** (e.g., capturing raw application click-stream user telemetry, live auditing logs, or massive AI vector ingestion flows) where millions of log strings cross the network wire per second and you need the ability to replay historical data streams.

---

### ❓ Q2: What happens if an Async Worker crashes halfway through executing its long-running database task? How does RabbitMQ prevent data loss?

*   **The Answer:** RabbitMQ prevents data loss through its **Explicit Acknowledgment (`autoAck = false`) network contract**. 
*   **The Failure Mechanic:** When RabbitMQ hands a payload down an active TCP socket to a worker container, it marks that message state as `Unacknowledged`. If the worker process encounters a fatal crash or the hosting container gets forcefully terminated mid-execution, that physical TCP connection drops.
*   **The Recovery Loop:** RabbitMQ instantly detects the termination of the channel socket. It moves the specific payload state from `Unacknowledged` back to `Ready` at the front of the queue. If there are other HA competing worker containers online, RabbitMQ round-robins the message to them immediately. The data is never lost because the broker never received the closing `BasicAck` confirmation code block.

---

### ❓ Q3: How do you handle a "Poison Message"—a corrupted payload that crashes your worker application every single time it tries to process it?

*   **The Trap:** If you catch an exception inside your worker, log it, and blindly reject the message by sending it back to the same queue, you trigger an **Infinite Redelivery Storm**. The worker instantly pulls the same broken payload, crashes again, rejects it again, and spins your server CPU up to 100% until the app memory exhausts.
*   **The Enterprise Fix:** Implement a **Dead Letter Exchange (DLX)** pattern.
    1. Configure your production queue with an argument pointing to a secondary routing station called a Dead Letter Exchange: `x-dead-letter-exchange`.
    2. When an exception block catches a deserialization error or domain execution failure inside your C# `BackgroundService`, instruct the channel to issue a **Negative Acknowledgment with no requeue**: `_channel.BasicNack(deliveryTag, multiple: false, requeue: false)`.
    3. RabbitMQ catches the rejection, removes the corrupted bytes from the live data stream, and routes them over to an isolated **Dead Letter Queue (DLQ)**. This clears the pipeline so standard user traffic continues flowing, while engineers analyze the poisoned payload out-of-band.

---

### ❓ Q4: If you scale your worker container horizontally to 5 instances to handle high traffic, how do you prevent race conditions if multiple workers pull messages for the exact same database record at the same time?

*   **The Architectural Reality:** RabbitMQ distributes messages round-robin to competing consumers. It does *not* look at the contents of the bytes. If two different messages regarding `Tenant_Alpha` are processed concurrently by Worker 1 and Worker 2, you risk direct **Data Corruption or Concurrency Overwrites** inside your relational database context.
*   **The Defensive Fix Strategy:**
    1. **Optimistic Concurrency Control (OCC):** Add a tracking version column (`[Timestamp]` or `Guid`) to your database entities. When EF Core tries to execute `SaveChangesAsync()`, it verifies the version matches the read snapshot. If a competing worker modified the record while the current thread was sleeping, the database throws a `DbUpdateConcurrencyException`, allowing you to safely catch the error and retry the operation.
    2. **Idempotency Checks:** Every message *must* contain a unique transaction identity key (e.g., `MessageId = Guid`). Before executing any mutations, the worker checks an in-memory cache or a lightweight transactional lookup table (`ProcessedMessages`) to verify if that specific tracking token has already been marked as complete. If it exists, the worker skips the operation and issues an instant `BasicAck`.

---

### ❓ Q5: What is the "Outbox Pattern" and why is it essential when pairing a database transaction with a RabbitMQ event publish?

*   **The Trap (The Dual-Write Failure):** A developer saves a user profile to a database and immediately calls `channel.BasicPublish()` to broadcast a `"UserCreatedEvent"` to RabbitMQ. If the database save succeeds, but the network link to RabbitMQ drops a millisecond later, your database is updated but the rest of your microservice system never hears about it. Conversely, if you publish the event first, and the database save subsequently rolls back due to a validation error, your system sends out a false event notifications trail.
*   **The Outbox Fix Pattern:**
    1. You eliminate out-of-band network calls from your API controller completely.
    2. Inside the exact same SQL database transaction where you save your core business record, you insert a copy of the outgoing message payload into a dedicated tracking table called `OutboxMessages`. Because it uses the same database context transaction, it guarantees atomic success: **either both rows commit to disk, or everything rolls back.**
    3. An independent background worker thread or a CDC tool (Change Data Capture) constantly polls that single table, reads the un-sent message payloads, pushes them safely to RabbitMQ over a stabilized network connection, and marks the outbox rows as complete only *after* confirming the broker's receipt token.
