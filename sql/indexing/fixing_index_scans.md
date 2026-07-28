# 🚂 What Happens Inside the Query Optimizer

### ❌ The Non-SARGable Behavior (Index Scan)
When you write code that scrambles column attributes, the database engine cannot use the organized structure of your index. It is forced to look at every single page, row-by-row, to find matches.
                       [ Row 1 ] 🔍 Checks value...
  [ Index Scan ] ----> [ Row 2 ] 🔍 Checks value...
                       [ Row 3 ] 🔍 Checks value...

### ✅ The SARGable Behavior (Index Seek)
When you write clean, SARGable logic, the engine instantly reads the root of the B-Tree index map and performs a high-speed pinpoint search, jumping straight to the exact row locations.
  [ Index Seek ] ----> Jump straight to [ Row 734 ] 🎯 (Instant Match)

---

# ⚡ Ultimate Guide to SARGable Queries: Consolidated Interview Patterns

A query is **SARGable** (Search Argument Able) when the database engine can utilize a standard **B-Tree Index Seek** to jump straight to the data, rather than scanning the entire table or index row-by-row. 

By grouping similar mistakes together, we can reduce the cheat sheet down to **4 core conceptual patterns** that cover every major performance trap.

---

### Pattern 1: Column Scrambling (The Function & Math Trap)
**The Rule:** Never touch, wrap, modify, or perform math on a table column inside the `WHERE` clause. Doing so forces the engine to calculate that modification for every single row, causing a slow **Index Scan**. Always shift functions and math to the value/variable side instead.

* **❌ Non-SARGable (Forces Full Index Scans):**
  ```sql
  -- Trap A: Date Functions
  SELECT CandidateID FROM Candidates WHERE YEAR(SubmissionDate) = 2026;

  -- Trap B: String Splicing Functions
  SELECT LawyerID FROM Lawyers WHERE LEFT(Name, 3) = 'Har';

  -- Trap C: Math on Columns
  SELECT CheckID FROM Verifications WHERE CostZAR * 1.15 > 500.00;

  -- Trap D: Null Handling Functions
  SELECT ClientID FROM Clients WHERE ISNULL(Industry, 'Unknown') = 'Finance';

  -- Trap E: Date Truncation / Casting
  SELECT LogID FROM VerificationLogs WHERE CAST(LogTimestamp AS DATE) = '2026-07-28';

  -- Trap F: Column Concatenation
  SELECT LawyerID FROM Lawyers WHERE FirstName + ' ' + LastName = 'Harvey Specter';
  ```

* **✅ SARGable Rewrites (Enables High-Performance Index Seeks):**
  ```sql
  -- Fix A & E: Open explicit date/time boundary windows
  SELECT CandidateID FROM Candidates WHERE SubmissionDate >= '2026-01-01' AND SubmissionDate < '2027-01-01';
  SELECT LogID FROM VerificationLogs WHERE LogTimestamp >= '2026-07-28 00:00:00' AND LogTimestamp < '2026-07-29 00:00:00';

  -- Fix B: Convert to a trailing wildcard (B-Trees can read strings left-to-right)
  SELECT LawyerID FROM Lawyers WHERE Name LIKE 'Har%';

  -- Fix C: Isolate the column by moving the math to the value side
  SELECT CheckID FROM Verifications WHERE CostZAR > (500.00 / 1.15);

  -- Fix D: Expand function wrappers into native boolean logic
  SELECT ClientID FROM Clients WHERE Industry = 'Finance' OR (Industry IS NULL AND 'Unknown' = 'Finance');

  -- Fix F: Filter fields independently so individual indexes can be read
  SELECT LawyerID FROM Lawyers WHERE FirstName = 'Harvey' AND LastName = 'Specter';
  ```

---

### Pattern 2: Hidden Data Type Conversions
**The Rule:** Ensure your input literal data type matches the exact database column definition type. If you filter a string column (`VARCHAR`) with a raw number (`INT`), MS SQL Server silently runs an invisible conversion function behind the scenes, instantly killing SARGability.

* **❌ Non-SARGable (Implicit Conversion Scan):**
  ```sql
  SELECT CandidateID FROM Candidates WHERE NationalID = 9401015800083; -- NationalID is a VARCHAR column
  ```
* **✅ SARGable Rewrite (Type Match Seek):**
  ```sql
  SELECT CandidateID FROM Candidates WHERE NationalID = '9401015800083'; -- Matching types explicitly
  ```

---

### Pattern 3: Optimizer Confusion (`OR` Filters & Negations)
**The Rule:** Standard indexes are structurally built to track what *is* there, not what *isn't* there, and they struggle when tracking multiple columns across a single `OR`. Split or convert these paths to clear individual tracks.

* **❌ Non-SARGable (Forces Index Scan):**
  ```sql
  -- Negative filters face a scan because they must check every page to verify an absence
  SELECT ClientID FROM Clients WHERE Industry != 'Legal' AND Industry != 'Healthcare';

  -- Mixed-column OR filters confuse the optimizer, causing it to abandon indexes entirely
  SELECT CandidateID FROM Candidates WHERE FullName LIKE 'Alex%' OR SubmissionDate = '2026-07-28';
  ```
* **✅ SARGable Rewrite (Enables Index Seeks):**
  ```sql
  -- Use positive inclusion if the boundary array values are known
  SELECT ClientID FROM Clients WHERE Industry IN ('Finance', 'Tech', 'Retail', 'Manufacturing');

  -- Use UNION ALL to give the optimizer two simple queries with distinct index targets
  SELECT CandidateID, FullName FROM Candidates WHERE FullName LIKE 'Alex%'
  UNION ALL
  SELECT CandidateID, FullName FROM Candidates WHERE SubmissionDate = '2026-07-28' AND (FullName NOT LIKE 'Alex%' OR FullName IS NULL);
  ```

---

### Pattern 4: Indexing for Partitioned Window Functions
**The Rule:** While window ranking logic must sit inside a subquery or CTE due to execution ordering, it will still scan tables unless backed by an explicit database index layout. To make partitioning perform like an index seek, create a **Composite Index** matching your query sequence.

* **The Code Query:**
  ```sql
  WITH RankedChecks AS (
      SELECT CandidateID, CheckType, CostZAR,
             ROW_NUMBER() OVER(PARTITION BY CandidateID ORDER BY CostZAR DESC) as rnk
      FROM Verifications
  )
  SELECT CandidateID, CheckType, CostZAR FROM RankedChecks WHERE rnk = 1;
  ```
* **✅ The SARGable Optimization Action:**
  ```sql
  -- Maps the index footprint to mirror the window structure: PARTITION BY columns first, then ORDER BY columns
  CREATE INDEX IX_Verifications_SARG_Window 
  ON Verifications (CandidateID, CostZAR DESC) 
  INCLUDE (CheckType);
  ```
