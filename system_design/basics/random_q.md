whats a stateless app?

A stateless app is a system where the server does not store any user data, history, or session context between requests. Every request from a client must be completely self-contained, containing all the information required for the server to process it.

Benefits:

Horizontal Scaling: You can instantly add or remove servers to handle traffic spikes, since any server can process any incoming request.
High Availability: If a server crashes, users can be seamlessly rerouted to a healthy one without losing their session progress.
Lower Server Costs: The server consumes less memory because it does not need to store and maintain millions of active user sessions.
Simpler Maintenance: Deploying updates is easier because you can restart or replace servers without needing to migrate live session data.

Trade-offs

Larger Network Payloads: Because the server remembers nothing, the client must send authentication tokens and context data with every single request, increasing bandwidth usage.
Client-Side Complexity: The frontend application (browser or mobile app) bears the responsibility of securely storing tokens and managing application state.
Database Heavy: Since the server doesn't hold data in local memory, it must frequently query databases or external caches (like Redis) to verify permissions or retrieve user records.

what's Operational Overhead?

refers to the ongoing time, money, and human effort required to keep a software system running smoothly, safely, and reliably in production.

e.g. LLM-TriageAgent would be high operational overhead if i replaced polling with websockets

Whats High-throughput?

High-throughput refers to a system's ability to process a massive volume of data or requests within a specific timeframe. While "speed" usually refers to how fast a single task finishes (latency), throughput refers to how many tasks are completed altogether.

Think of high-throughput like a 10-lane highway:It is not built to make a single sports car travel faster.It is built to move thousands of trucks past a checkpoint every hour.

Benefits:
Massive Concurrency: The system handles thousands or millions of concurrent users without crashing.