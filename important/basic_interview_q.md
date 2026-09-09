1. What is the difference between IEnumerable, IQueryable and IAsyncEnumerable?

2️⃣ Why can IQueryable become dangerous when used incorrectly with EF Core?

3️⃣ Scoped vs Singleton vs Transient — when would you use each?

4️⃣ What happens when you inject a Scoped service into a Singleton?

"You create a Captive Dependency, which traps the Scoped service in memory forever.
"Because the Singleton never dies, it leaks the Scoped reference into the entire lifecycle of the application, completely destroying the request isolation boundary.
If that captured service is a DbContext, it forces concurrent threads to share a single state—triggering immediate memory leaks, multithreading race conditions, and database crashes.

### 🚨 The Captive Dependency: The Water Poisoning Fallacy

* **The Analogy:** Injecting a Scoped service into a Singleton is like dumping a temporary **Local Jug of Water** directly into the permanent **Central Passage Geyser**. 
* **The Reality:** The Geyser never dies, so it traps that specific jug of water inside its tank forever, completely destroying the fresh request isolation boundary.
* **The Crash:** If that jug was a database `DbContext`, every tap in the house (concurrent user threads) is forced to share the exact same water supply simultaneously, triggering immediate resource contamination and system-wide blackouts.


5️⃣ Middleware vs Action Filter vs Endpoint Filter — what's the real difference?

6️⃣ Why does async/await not automatically create a new thread?

7️⃣ What causes ThreadPool starvation in ASP.NET Core?

8️⃣ Task.WhenAll vs Parallel.ForEachAsync — when should you use each?

9️⃣ How does EF Core tracking actually work?

🔟 When would you choose Dapper over EF Core?
