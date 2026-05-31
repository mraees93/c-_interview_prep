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
