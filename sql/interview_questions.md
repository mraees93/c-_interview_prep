# SQL & Relational Database Mechanics - Interview Preparation

This module tracks query optimizations, data grouping pipelines, constraint enforcement, and performance scaling rules for processing multi-million row transactional databases.

---

## 1. Table Joins & Null Handling

### The Panel Scenario
An interviewer hands you a database schema for an entity tracking court cases and assigned judges. They ask you to extract a report showing *all* cases alongside their assigned judge's name, but you must ensure that cases without an assigned judge are still included in the final dataset rather than dropped.

```sql
-- Schema Context:
-- Cases Table: CaseId, Title, JudgeId (Nullable)
-- Judges Table: JudgeId, FullName
```

### Questions & Core Answers
*   **Q1: Which JOIN type achieves this requirement?**
    *   **Answer**: A **`LEFT JOIN`** (or `LEFT OUTER JOIN`). It preserves every row from the left table (`Cases`) regardless of whether a matching record exists in the right table (`Judges`). A standard `INNER JOIN` would drop any case where `JudgeId IS NULL`.
*   **The Refactored Fix**:
```sql
SELECT c.CaseId, c.Title, COALESCE(j.FullName, 'Unassigned') AS JudgeName
FROM Cases c
LEFT JOIN Judges j ON c.JudgeId = j.JudgeId;
```
*   *Interview Pro-Tip*: Utilizing `COALESCE` or `ISNULL` to handle the empty fields cleanly shows intermediate-level attention to clean output data structures.

---

## 2. Aggregation Filtering: `WHERE` vs. `HAVING`

### The Panel Question
What is the precise structural difference between a `WHERE` clause and a `HAVING` clause, and can you use an aggregate function like `COUNT()` inside a `WHERE` statement?

### Core Answer
*   **Execution Sequence**: The SQL engine runs the `WHERE` clause **before** rows are grouped via `GROUP BY`. It filters individual raw rows. The `HAVING` clause runs **after** the grouping occurs, filtering the aggregated summary buckets.
*   **The Rule**: You **cannot** use an aggregate function like `COUNT()`, `SUM()`, or `AVG()` inside a `WHERE` clause because individual row evaluations lack context about group summaries. Group condition checks must reside exclusively within the `HAVING` statement.
```sql
-- CORRECT PATTERN
SELECT DepartmentId, COUNT(*) AS TotalEmployees
FROM Employees
WHERE Salary > 50000 -- Filters raw rows first
GROUP BY DepartmentId
HAVING COUNT(*) > 5; -- Filters grouped summaries last
```

---

## 3. Database Indexes (Performance Optimization)

### The Panel Scenario
A background API query that searches legal documents by a string identifier (`DocumentHash`) has become extremely slow as the database table scales to 10 million records. The query executes hundreds of times a minute:
`SELECT * FROM Documents WHERE DocumentHash = 'A9B8C7';`

### Questions & Core Answers
*   **Q1: How do you fix this performance bottleneck under the hood?**
    *   **Answer**: Create a **Non-Clustered Index** on the `DocumentHash` column. 
*   **Q2: How does an index change the query search complexity mathematically?**
    *   **Answer**: Without an index, the SQL engine executes a **Table Scan** with a time complexity of **O(N)**, searching through every single row manually. An index structures the column data into a balanced tree pointer graph (**B-Tree**), converting the search execution into an **Index Seek** with a logarithmic time complexity of **O(log N)**, making it virtually instant.

---

## 4. Subqueries vs. Common Table Expressions (CTEs)

### The Panel Question
What is a Common Table Expression (CTE), and why do enterprise developers choose them over deeply nested subqueries?

### Core Answer
*   **The Definition**: A CTE is a temporary, named result set defined using the `WITH` statement that exists solely within the execution scope of a single query.
*   **The Benefits**: Deeply nested subqueries become unreadable and extremely difficult to maintain or debug. CTEs drastically improve code readability by allowing you to break complex data transformations into sequential, logical blocks.
```sql
-- Clear, Readable CTE Pattern
WITH RecentHighValueOrders AS (
    SELECT CustomerId, OrderTotal
    FROM Orders
    WHERE OrderDate > '2026-01-01' AND OrderTotal > 10000
)
SELECT c.CustomerName, o.OrderTotal
FROM Customers c
JOIN RecentHighValueOrders o ON c.CustomerId = o.CustomerId;
```
