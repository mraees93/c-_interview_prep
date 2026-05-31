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
