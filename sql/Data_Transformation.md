Focuses on active logic queries, shaping tables, filtering partitions, and combining records (Inner/Left Joins, CTEs, Window Functions, and Aggregate groups).

# Relational Data Shaping: Joins, Aggregates & Window Functions

This guide serves as a rapid-review index for data manipulation, relational merging, time-series deduplication, and logical query compilation.

---

## 1. The Join Boundary Rules

When combining entity tables, you must explicitly state how the query engine should handle missing structural keys.

*   **INNER JOIN**: Extracts rows only when a matching structural key exists in both tables. Used when strict relational integrity is mandatory [Scenario 3].
*   **LEFT (OUTER) JOIN**: Returns 100% of the rows from the primary left table, alongside matching rows from the right table. If no match exists on the right, those columns are filled with `NULL` tokens. 
    *   *Enterprise Use Case:* Fetching a master list of legal cases where some records may not have active revision histories or attachments yet.
*   **CROSS JOIN**: Generates a complete Cartesian product ($O(N \times M)$ mapping every single row of Table A to every row of Table B). 
    *   *Warning:* Never execute this on massive tables in production, as it triggers a severe database memory explosion.

### 🚩 THE LEFT JOIN CRITICAL INTERVIEW TRAP

An interviewer will show you a query looking for items that *do not* exist in a secondary table. They will hide a filter mistake inside the `WHERE` clause to see if you catch it.

```sql
-- ❌ THE TRAP QUERY: Suppressing the LEFT JOIN
-- The goal is to find Judges who do not have any cases assigned.
SELECT j.Id, j.Name
FROM Judges j
LEFT JOIN LegalCases c ON j.Id = c.JudgeId
WHERE c.Status = 'Active' OR c.Status IS NULL; 
```

#### ❓ The Interviewer's Question
> *"Looking at this query, will it correctly return all Judges who have no cases assigned? If not, why is it broken and how do you fix it?"*

#### 🟢 Your Winning Answer
*"No, this query is broken. It will completely strip out any Judges who do not have cases. Because the `WHERE` clause filter explicitly evaluates `c.Status = 'Active'`, it forces the database engine to search for real values inside the right-hand table (`LegalCases`). This evaluation silently **converts your LEFT JOIN into an INNER JOIN**, wiping out all unmatched rows where `c.Status` is natively `NULL`.*

*To fix this trap, any filtering filters on the optional right-hand table must live strictly inside the **`ON` clause** of the join, ensuring the data is filtered **before** the table rows are merged."*

```sql
-- 🟢 THE CORRECT REFACTOR
SELECT j.Id, j.Name
FROM Judges j
LEFT JOIN LegalCases c ON j.Id = c.JudgeId AND c.Status = 'Active'
WHERE c.JudgeId IS NULL; -- Safely filters for unmatched rows
```

---

## 2. Aggregates vs. Window Functions

Understanding how data rows flow through memory defines the difference between a standard grouping and a window partition.

*   **Standard Aggregates (`GROUP BY`)**: Collapses your individual data records down into a single consolidated summary row (e.g., counting total legal filings per judge) [Scenario 4]. You lose the ability to see individual row identities or unique metadata in the final output stream.
*   **Window Functions (`OVER()`)**: Executes mathematical or ranking calculations across an isolated subset of rows related to the current record, **without collapsing the rows**. Every single original data row retains its unique identity and properties in the output view.

### 🚩 THE WHERE CLAUSE LOGICAL TIMING TRAP

```sql
-- ❌ THE TRAP QUERY (Triggers a Compilation Error)
SELECT 
    CaseId, Title,
    ROW_NUMBER() OVER(PARTITION BY JudgeId ORDER BY ChangeDate DESC) as RowNum
FROM LegalCases
WHERE RowNum = 1; -- 💥 COMPILATION ERROR: Invalid column name 'RowNum'
```

#### ❓ The Interviewer's Question
> *"Why does this query crash with a compilation error, and how do you circumvent it?"*

#### 🟢 Your Winning Answer
*"This query crashes because of the **Logical Order of Query Execution**. SQL Server does not read a query from top-to-bottom. The compilation steps compile in this exact sequence:*

`1. FROM` ➔ `2. ON` ➔ `3. JOIN` ➔ `4. WHERE` ➔ `5. GROUP BY` ➔ `6. HAVING` ➔ `7. SELECT` ➔ `8. DISTINCT` ➔ `9. ORDER BY`

*Because the `WHERE` clause compiles at step 4, but window functions compute inside the `SELECT` phase at step 7, the query engine has no physical knowledge that `RowNum` exists yet when evaluating the filter. To fix this, we must wrap the execution inside a **Common Table Expression (CTE)** to force the window function to materialize its columns first."*

```sql
-- 🟢 THE CORRECT CTE REFACTOR
WITH RankedCases AS (
    SELECT 
        CaseId, Title,
        ROW_NUMBER() OVER(PARTITION BY JudgeId ORDER BY ChangeDate DESC) as RowNum
    FROM LegalCases
)
SELECT CaseId, Title
FROM RankedCases
WHERE RowNum = 1; -- Safely filters because the CTE initialized the schema step
```

---

## 3. High-Frequency Interview Challenge: Deduplication & Version Controls

A classic corporate task at LexisNexis is finding the "most recent update," "latest revision," or "top entry" inside a specific sub-category. 

### ❓ The Interviewer's Question
> *"We have a `CaseAuditLogs` table tracking multiple chronological updates for our legal cases. Write a query that extracts only the absolute latest update row for every unique case, keeping all original row metadata intact."*

```sql
-- 🟢 THE PRODUCTION IMPLEMENTATION
WITH RankedUpdates AS (
    SELECT 
        CaseId, UpdateText, ChangeDate, OperatorName,
        -- PARTITION BY splits data into isolated data buckets per Case
        -- ORDER BY dictates how the internal index counter increments
        ROW_NUMBER() OVER(PARTITION BY CaseId ORDER BY ChangeDate DESC) as RowNum
    FROM CaseAuditLogs
)
SELECT CaseId, UpdateText, ChangeDate, OperatorName
FROM RankedUpdates
WHERE RowNum = 1; -- RowNum = 1 guarantees we pull only the absolute latest log row
```

#### ❓ The Tie-Breaker Follow-up
> *"What is the exact mechanical difference between `ROW_NUMBER()`, `RANK()`, and `DENSE_RANK()`, and how do they behave if two records have identical `ChangeDate` values?"*

#### 🟢 Your Winning Answer
*"They handle matching duplicates (ties) across three distinct index allocation strategies:*
*   `ROW_NUMBER()` *is strictly sequential. It completely ignores ties and assigns a unique, ascending integer value (`1, 2, 3, 4`) arbitrarily to the rows.*
*   `RANK()` *acknowledges the tie but leaves matching gaps. If the top two dates tie, both receive an index value of `1`, but the third record skips directly to an index position of `3` (`1, 1, 3, 4`).*
*   `DENSE_RANK()` *acknowledges the tie without leaving gaps. If the top two dates tie, both receive an index value of `1`, and the third record is assigned an sequential index value of `2` (`1, 1, 2, 3`)."*

---

## 4. Analytical Time Series: Lead vs. Lag

LexisNexis analytical reporting relies heavily on tracking historical shifts (e.g., tracking how a document's status changed over time or comparing month-over-month telemetry trends).

### ❓ The Interviewer's Question
> *"Write a query that looks at the current row's update value and displays the previous row's update value side-by-side in the same query stream output."*

```sql
-- 🟢 THE TIME-SERIES DELTA PATTERN
SELECT 
    CaseId,
    ChangeDate,
    Status as CurrentStatus,
    -- LAG looks backward into the partition stream layout
    LAG(Status, 1, 'Initial Creation') OVER(PARTITION BY CaseId ORDER BY ChangeDate ASC) as PreviousStatus,
    -- LEAD looks forward into the upcoming partition stream
    LEAD(Status, 1) OVER(PARTITION BY CaseId ORDER BY ChangeDate ASC) as NextStatus
FROM CaseAuditLogs;
```

#### 🧠 The Talking Point to Use:
*"By passing an optional third argument into `LAG(Status, 1, 'Initial Creation')`, I establish a safe default value. If a legal case is on its very first row entry, looking backward would normally return a `NULL` token. Providing a string literal default fallback keeps our analytical payload clean and friendly for our React application parsing layers."*
