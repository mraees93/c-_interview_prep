# ORM & Data Access (EF Core)

## 1. IQueryable vs. IEnumerable (The Execution Boundary)
* IQueryable = Database-side execution. Generates an Expression Tree, compiles to SQL, and filters on the DB. Only matching rows cross the network.
* IEnumerable = Application-side execution. Pulls the entire raw dataset into web server memory (SELECT *), then filters using the C# CLR. 
* Interview Tip: Mixing these up on large legal datasets causes memory bloat and application crashes.

## 2. AsNoTracking() (Memory Optimization)
* By default, EF Core keeps a copy of data in memory (Change Tracker) to look for updates.
* .AsNoTracking() turns off the Change Tracker. 
* Result: Halves memory usage and speeds up read-only queries by up to 2x.
* Golden Rule: If the endpoint doesn't call _context.SaveChanges(), use .AsNoTracking().

## 3. The N+1 Query Problem
* Cause: Fetching a parent list and looping through it to fetch child records individually.
* Fix: Use Eager Loading (.Include()) or explicit Projection (.Select() into a DTO) to force EF Core to fetch all data in a single, clean SQL JOIN.


### 📂 Data Access Performance Modules

#### 1. File: `QueryExecutionService.cs` (Folder: `QueryMechanics`)
*   **The Execution Boundary (`IEnumerable` vs. `IQueryable`)**: Proves you know *where* code executes. Highlights the danger of pulling entire database tables into web server RAM versus using the SQL Engine to filter data before it hits the network wire.
*   **Memory Management (`.AsNoTracking()`)**: Teaches you how to explicitly disable Entity Framework's internal "Change Tracker" audit engine for read-only pages, cutting API server RAM usage by up to 50%.

#### 2. File: `DataProjectionService.cs` (Folder: `DataProjections`)
*   **Network Latency Optimization (The N+1 Problem)**: Focuses on spotting and destroying database queries hidden inside loop structures (`foreach`) that freeze production environments.
*   **Database Join Mechanics (DTO Projection)**: Teaches how to use `.Select()` to force a single SQL `JOIN`, retrieve only the exact columns needed, and **implicitly activate No-Tracking optimizations** automatically.



### 💡 The Core Memory Rule: IQueryable vs. IEnumerable

*   **Golden Rule for Database Lookups**: 
    Always keep the collection as an `IQueryable` while building your query to ensure all filtering, sorting, and paging happen directly on the SQL Database Engine. 
    Only transition to an `IEnumerable` (or concrete list) once the data has safely crossed the network boundary into the web server's memory.

#### 🔍 When exactly does data enter the Web Server's Memory?

Data moves from the database server disk/cache into your Web Server's RAM (terminating the `IQueryable` pipeline) under two specific conditions:

1.  **Explicit Materialisation (The Right Way)**
    *   **How it happens**: When you call a "terminal/materialisation method" at the end of an `IQueryable` chain.
    *   **The Methods**: `ToListAsync()`, `FirstAsync()`, `CountAsync()`, `AnyAsync()`, or their synchronous equivalents (`ToList()`, `First()`).
    *   **Why it's safe**: The database executes the highly filtered SQL query first, packs up *only* the matching rows, sends them over the network wire, and EF Core instantiates them into memory as a clean C# collection.

2.  **Implicit/Premature Streaming (The Dangerous Way)**
    *   **How it happens**: When you cast a database query pool directly to an `IEnumerable` or evaluate it using non-SQL logic *before* filtering.
    *   **The Methods**: Calling `.AsEnumerable()` mid-chain, casting to `(IEnumerable<T>)_context.Table`, or running a `foreach` loop directly over an unmaterialised `DbSet`.
    *   **Why it's dangerous**: This forces an immediate, un-indexed `SELECT * FROM Table` behind the scenes. The database dumps the entire raw table over the network, flooding your web server's RAM with unfiltered records, which forces your local C# threads to do the filtering manually.
