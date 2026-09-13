in a join question how do i differentiate between using an inner join or left join?

I know when they say the right columns data can be empty/null or they refer to it


1. The "Every/All" Rule (LEFT JOIN)
If the question asks for "all" of one category, regardless of whether they have a relationship, use a LEFT JOIN.
Keywords: "All lawyers, even those without cases," "Every department, including empty ones," "List all users and their orders (if any)."
Logic: You are protecting the data in the "Left" table from being filtered out.
2. The "Only/Matching" Rule (INNER JOIN)
If the question asks for a list where both sides must exist, use an INNER JOIN.
Keywords: "List matters and their lawyers," "Show me documents assigned to a matter," "Who is the lawyer for Case X?"
Logic: If a Matter doesn't have a Lawyer, it shouldn't show up at all.
3. The "Missing/Not In" Rule (LEFT JOIN + NULL)
If the question asks you to find things that are missing or unassigned, it is always a LEFT JOIN.
Keywords: "Which lawyers have no matters?", "Find matters with zero documents."
Logic: You join them, then look for the "empty" spots where the match failed.
4. The "Integrity" check
If you know for a fact (via the schema) that a column is NOT NULL (mandatory), an INNER JOIN and LEFT JOIN will actually return the exact same result.
Example: If every Matter must have a LeadLawyerID, then Matters JOIN Lawyers is the same as Matters LEFT JOIN Lawyers.

a LEFT JOIN is more "expensive" for the database than an INNER JOIN, and that you only use it when strictly necessary.



Tip:
If they ask you to filter based on an aggregate (Sum, Count, Avg), always reach for HAVING. If they ask you to filter based on a specific row property, use WHERE.
If an interviewer asks you to filter based on a number of occurrences (like "more than 1", "at least 5"), your brain should immediately think: "Group By + Having Count."

-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--7. Find the CompanyName of clients whose average verification check cost (CostZAR) across all their candidates is strictly greater than 200.00.



The Bottom Line:
Mastering Inner and Left Joins is 80% of the battle. The other 20%—which is what the interview focuses on—is knowing when to use which one and how to keep the query fast as the database grows to millions of records.



tips to avoid duplicates:

- GROUP BY the column id
- select DISTINCT





Rule of Thumb: Use SUM for values (money, weight, sizes) and COUNT for tracking "how many" items there are.

Using COUNT(Quantity) instead of SUM(Quantity) is one of the most common mistakes candidates make when transitioning from Junior to Intermediate roles.
Here is why that mistake changes the logic of your code, and how to keep it straight in your notes:The Difference in Output

Let's look at a single order that contains two items:
Item 1: 2 laptops
Item 2: 5 mice

Function:            How the Database Thinks:                                                   Your Result: 
COUNT(Quantity)    "How many rows or entries are there in this group?"                         2 (Because there are two separate rows of items)
SUM(Quantity)      "What is the math total of the numbers in this column?"               7 (\(2 + 5\) total items shipped)


If the question asks for a specific metric threshold (e.g., HAVING SUM(x) > 500 or HAVING AVG(y) < 50), use an INNER JOIN to optimize performance.



Use DISTINCT when you are jumping "up" a relationship to find unique parents (e.g., "Find unique CustomerNames from the Orders table").
Do NOT use DISTINCT when you are listing items from a base inventory table (like Products), because each individual item should be allowed to show up on its own row.
refer to practice2.sql file answer 6

You have to use GROUP BY when you use an aggregate function like COUNT(), SUM(), or AVG() alongside a regular column (like m.Title), SQL needs to know how to group the individual rows.




In SQL, the physical order of your JOIN statements dictates what table aliases are available. You can only use a table's alias if that table has already been introduced in the query above or on that exact line.

Broken Chain: Orders \(\rightarrow \) OrderItems \(\rightarrow \) [Tries to use Products column] \(\rightarrow \) Categories \(\rightarrow \) Products
Correct Chain: Orders \(\rightarrow \) OrderItems \(\rightarrow \) Products \(\rightarrow \) Categories

-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

--4. List the Names of Customers who have ordered products from the 'Electronics' category.

SELECT DISTINCT o.CustomerName
FROM Orders o 
JOIN OrderItems oi ON o.OrderID = oi.OrderID
JOIN Categories c ON c.CategoryID = p.CategoryID
JOIN Products p ON c.CategoryID = p.CategoryID
WHERE c.CategoryName = 'Electronics';

SELECT DISTINCT o.CustomerName
FROM Orders o
JOIN OrderItems oi ON o.OrderID = oi.OrderID
JOIN Products p ON oi.ProductID = p.ProductID
JOIN Categories c ON c.CategoryID = p.CategoryID
WHERE c.CategoryName = 'Electronics';



https://www.youtube.com/watch?v=rIcB4zMYMas
Window functions:

Unlike regular aggregate functions, window functions do not collapse rows into a single output; they keep the identity of each row intact

The function does the math or counting, while the OVER() clause defines who is included in that calculation.

The OVER clause defines the 'window' (or subset) of data the function calculates across, allowing you to run aggregate math without collapsing your individual rows.

SELECT Gender, Name, Total

    //Function                            //Window
    //1..2..3 in popularity column rows   // how you wanna view your data when applying your function
    ROW_NUMBER()                          OVER(ORDER BY Total DESC)                                       AS Popularity

FROM baby_names



//split the gender column into boy and girl group vertically in the gender column - use PARTITION BY

SELECT Gender, Name, Total

    //Function                            //Window
    //1..2..3 in popularity column rows   // how you wanna view your data when applying your function
    ROW_NUMBER()                          OVER(PARTITION BY Gender ORDER BY Total DESC)                                       AS Popularity
    
FROM baby_names


-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--9. Write a query to show the CandidateID, CheckType, and CostZAR, alongside a new column displaying the average cost of that specific CheckType across the entire 
-- database.



-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

----5. List all Lawyers and any Matters they lead with 'Litigation' in the title. Lawyers with no 'Litigation' matters must still 
-- appear in the list.

-------WRONG----------
SELECT l.Name, m.Title
FROM Lawyers l 
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
WHERE m.Title = 'Litigation'

SELECT l.Name, m.Title
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID 
     **AND m.Title LIKE '%Litigation%';**

--Filter in ON: Filters the "right" table before the join. Keeps all rows from the "left" table.
--Filter in WHERE: Filters the entire result after the join. Can accidentally delete "left" table rows.

**putting '' around a string value is used to match an exact text string, 
Operator used: Equals sign (=)
Performance: Ultra-fast, especially if the column is indexed.

**putting percentage signs (%) inside quotation marks is a wildcard tool used for partial text matching (searching).
Operator used: You must use the LIKE operator. If you use = with percentage signs (e.g., WHERE Title = '%Hardware%'), SQL will literally search for text containing actual percent signs.
Performance: Slower. It forces SQL Server to perform a full-table scan (reading every single row) because it cannot use standard index sorting trees effectively.

**How to optimize a wildcard partial text matching query?**

**This QUERY doesnt make it truly SARGable in the true sense but it achieves the exact same ultimate goal: high performance without a full table scan.**

SELECT l.Name, m.Title
FROM Lawyers l 
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
AND CONTAINS(m.Title, 'Litigation'); -- This utilizes an MS SQL Full-Text Index and is highly performant

**normally, wrapping a column inside a function completely destroys SARGability and forces an Index Scan. CONTAINS() is the one major exception to this rule because it does not use standard database indexes it uses Microsoft SQL Server's Full-Text Search (FTS) engine.**

-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

--5. Find Orders where the total quantity of items is greater than 10

SELECT oi.OrderID, COUNT(oi.Quantity) AS QuantityCount
FROM OrderItems oi
GROUP BY oi.OrderID
HAVING COUNT(oi.Quantity) > 10;
-- didnt have to join here despite answer query having a join. Because OrderItems table contains OrderID and Quantity
-- If an interviewer gives you a question like Q5 and you catch this shortcut, you immediately stand out as an Intermediate Engineer 
-- because you are actively thinking about Performance and Efficiency.
-- Cuts database work in half. It reads from a single table, bypassing the expensive join overhead entirely.



-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--1. Find the FullName and SubmissionDate of all candidates who submitted their details in January 2026.

SELECT FullName, SubmissionDate
FROM Candidates
WHERE SubmissionDate >= '2026-01-01'
    AND SubmissionDate < '2026-02-01';

Using an asymmetrical range (>= '2026-01-01' AND < '2026-02-01') instead of a LIKE operator or pulling the month out via a function (like MONTH()) is the exact engineering best practice LexisNexis looks for. It keeps the query sargable, meaning the database engine can fully utilize an index on the SubmissionDate column.



-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--6. Return the CandidateID and FullName of any candidate who has at least one check with a 'Pending' status, but completely exclude candidates who have any checks 
-- with a 'Flagged' status.

--my try:
-- SELECT c.CandidateID, c.FullName
-- FROM Candidates c 
-- JOIN Verifications v ON c.CandidateID = v.CandidateID
-- GROUP BY c.CandidateID, c.FullName
-- HAVING NOT v.Status;
-- WHERE v.Status = 'Pending' AND v.Status != 'Flagged';

--shortest solution i found
SELECT DISTINCT 
    c.CandidateID, 
    c.FullName
FROM Candidates c
JOIN Verifications p ON c.CandidateID = p.CandidateID AND p.Status = 'Pending' -- filters candidate pool to only include individuals who have at least one 'Pending' check
LEFT JOIN Verifications f ON c.CandidateID = f.CandidateID AND f.Status = 'Flagged' --find only the 'Flagged' checks for those exact same candidates.
WHERE f.CandidateID IS NULL; --drop anyone who has a flagged status


# SQL Query Execution Order Reference

This reference guide maps out the difference between how you **write** an SQL query versus how the database engine actually **executes** it. Understanding this distinction is critical for debugging queries and acing technical interviews.

---

## 🚀 The Core Logical Execution Order

When a database processes your query, it follows this strict step-by-step sequence:

1. **`FROM` / `JOIN`**
   - The engine identifies the target tables and combines them to form the base dataset.
2. **`ON`**
   - Join conditions are evaluated to filter the combined rows.
3. **`WHERE`**
   - Base rows are filtered by conditions *before* any grouping or aggregation occurs.
4. **`GROUP BY`**
   - The remaining rows are collapsed into summary rows based on matching column values.
5. **`HAVING`**
   - Conditions are applied to filter the aggregated groups (unlike `WHERE`, which filters raw rows).
6. **`SELECT`**
   - The database determines which columns to return, calculates expressions, and applies aliases.
7. **`DISTINCT`**
   - Duplicate rows are removed from the selected output dataset.
8. **`ORDER BY`**
   - The final result set is sorted sequentially by the specified columns.
9. **`LIMIT` / `OFFSET` / `TOP`**
   - The dataset is restricted to a maximum number of rows for presentation or pagination.

---

## 📊 Summary Comparison Matrix

| Step | Written Order (Syntax) | Execution Order (Logical) | Interview Crucial Note |
| :--- | :--- | :--- | :--- |
| **1** | `SELECT` | `FROM` / `JOIN` | Base dataset must exist before anything else. |
| **2** | `FROM` / `JOIN` | `WHERE` | Filters rows early to save computing power. |
| **3** | `WHERE` | `GROUP BY` | Collapses rows; standard column data is lost unless aggregated. |
| **4** | `GROUP BY` | `HAVING` | Filters aggregate results (e.g., `COUNT(*) > 5`). |
| **5** | `HAVING` | `SELECT` | Evaluates expressions and assigns `AS` aliases. |
| **6** | `ORDER BY` | `DISTINCT` | Deduplicates data right before final presentation. |
| **7** | `LIMIT` / `TOP` | `ORDER BY` | Sorts data so that limits fetch the correct "top" rows. |
| **8** | — | `LIMIT` / `OFFSET` | Cuts the final feed to the exact pagination size requested. |

---

## 💡 Practical Interview Traps & Pitfalls

### ❌ Trap 1: Using Aliases in the `WHERE` Clause
This query fails because the engine runs `WHERE` **before** it processes `SELECT` (where the alias is created).
```sql
-- INVALID QUERY
SELECT DepartmentID AS Dept, AVG(Salary) AS AvgSal
FROM Employees
WHERE Dept = 10; -- Error: "Dept" does not exist yet!
```

### ✅ The Fix
You must repeat the literal column name in the `WHERE` clause, or use a Subquery / Common Table Expression (CTE).
```sql
-- VALID QUERY
SELECT DepartmentID AS Dept, AVG(Salary) AS AvgSal
FROM Employees
WHERE DepartmentID = 10; 
```

### ❌ Trap 2: Using Aggregate Functions in the `WHERE` Clause
This query fails because individual rows are filtered **before** the engine groups or aggregates them together.
```sql
-- INVALID QUERY
SELECT DepartmentID, AVG(Salary)
FROM Employees
WHERE AVG(Salary) > 50000 -- Error: An aggregate may not appear in the WHERE clause
GROUP BY DepartmentID;
```

### ✅ The Fix
Filter aggregated groups using the `HAVING` clause, which executes **after** `GROUP BY`.
```sql
-- VALID QUERY
SELECT DepartmentID, AVG(Salary)
FROM Employees
GROUP BY DepartmentID
HAVING AVG(Salary) > 50000;
```

### ❌ Trap 3: Structural Aliasing Limits in `GROUP BY`
Trying to group by a newly calculated column alias declared in the `SELECT` statement (specifically in MS SQL Server).
* **Why it breaks:** The engine groups data at Step 4, but your `SELECT` aliases are not evaluated or assigned until Step 6. PostgreSQL and MySQL have compiler workarounds for this, but MS SQL Server strictly adheres to the standard and throws an exception.
```sql
-- 🚫 FAILS (In MS SQL Server)
SELECT CASE WHEN Score > 80 THEN 'High Risk' ELSE 'Low Risk' END AS RiskTier, COUNT(*)
FROM RiskAssessments
GROUP BY RiskTier; -- Error: Invalid column name 'RiskTier'

-- ✅ FIXED (Repeat the expression or wrap it in a CTE)
SELECT CASE WHEN Score > 80 THEN 'High Risk' ELSE 'Low Risk' END AS RiskTier, COUNT(*)
FROM RiskAssessments
GROUP BY CASE WHEN Score > 80 THEN 'High Risk' ELSE 'Low Risk' END;
```

### ❌ Trap 4: Non-Deterministic `DISTINCT` + `ORDER BY` Mismatch
Using `SELECT DISTINCT` while ordering your result set by a column that was not explicitly included in the `SELECT` list.
* **Why it breaks:** `DISTINCT` runs at Step 7 to collapse duplicate rows. `ORDER BY` runs later at Step 8. If multiple rows have identical selected data but different values in the sorting column, the engine does not know which sorting value to prioritize, creating a mathematical ambiguity.
```sql
-- 🚫 FAILS
SELECT DISTINCT Country
FROM LegalEntities
ORDER BY DateCreated DESC; -- Error: ORDER BY items must appear in the select list if SELECT DISTINCT is specified

-- ✅ FIXED
SELECT Country, MAX(DateCreated) as MaxDate
FROM LegalEntities
GROUP BY Country
ORDER BY MaxDate DESC;
```


# 🚀 Advanced SQL Execution Order: Part 2 (Software Engineer Focus)
### 🎯 Target: LexisNexis Cape Town — Intermediate SE Assessment

This document houses the advanced architectural and optimization traps where the database engine's logical execution pipeline can cause production performance degradation or silent application logic bugs.

---

## ⚠️ Additional Intermediate Software Engineering Traps

### ❌ Trap 5: The `COUNT(column)` vs. `COUNT(*)` Null Mirage
Assuming `COUNT(ColumnName)` and `COUNT(*)` do the exact same thing when calculating aggregates.
* **Why it breaks:** During Step 4 (`GROUP BY`) and Step 6 (`SELECT`), the engine allocates memory differently for these functions. `COUNT(*)` counts every row matching the criteria (including rows where all columns are null). `COUNT(ColumnName)` looks explicitly at that column and silently discards any row containing a `NULL`.
* **The Impact:** This creates critical bugs in application metrics, billing code calculations, or payment volume tracking if columns contain optional parameters.
```sql
-- 🚫 POTENTIAL APPLICATION BUG
SELECT DepartmentId, COUNT(ManagerId) as TotalStaff 
FROM Teams 
GROUP BY DepartmentId; -- Missing all staff members who do not have a manager assigned!

-- ✅ FIXED (Counts actual transaction/row records defensively)
SELECT DepartmentId, COUNT(*) as TotalStaff 
FROM Teams 
GROUP BY DepartmentId;
```

### ❌ Trap 6: Subquery Unnesting & The `NOT IN` Black Hole
Using a standard `NOT IN` condition against a dynamic subquery to filter records.
* **Why it breaks:** If the subquery executed in Step 1/3 returns even a single `NULL` value, the entire `NOT IN` expression evaluates to `UNKNOWN`. Due to SQL's three-valued logic, the database engine will return zero rows for the entire query.
```sql
-- 🚫 DANGEROUS (If any user has a NULL Email, this returns exactly 0 results)
SELECT AppId FROM Applications 
WHERE UserEmail NOT IN (SELECT Email FROM BlacklistedUsers);

-- ✅ FIXED (Use NOT EXISTS, which uses two-valued boolean logic safely)
SELECT a.AppId FROM Applications a
WHERE NOT EXISTS (
    SELECT 1 FROM BlacklistedUsers b WHERE b.Email = a.UserEmail
);
```

### ❌ Trap 7: Non-SARGable Expressions Wiping Out Indexes
Wrapping an indexed table column inside an inline function (like `DATEPART`, `LEFT`, or `CONVERT`) inside your filter constraints.
* **Why it breaks:** To process a filter at Step 3 (`WHERE`), the engine needs to evaluate the function for *every single row* in the database sequentially before it can compare it. This completely prevents the Query Optimizer from performing a rapid **Index Seek**.
* **The Impact:** A query that should take 2 milliseconds ends up executing a massive **Full Table Scan**, locking production database tables at scale.
```sql
-- 🚫 PERFORMANCE NIGHTMARE (Forces Table Scan)
SELECT CaseId FROM LegalCases 
WHERE YEAR(ClosedDate) = 2026;

-- ✅ FIXED (SARGable: Allows the engine to use a B-Tree Index Seek)
SELECT CaseId FROM LegalCases 
WHERE ClosedDate >= '2026-01-01' AND ClosedDate < '2027-01-01';
```

### ❌ Trap 8: The Accumulator Illusion with Window Functions
Omitting the `ORDER BY` clause inside a cumulative Window Function partition and expecting a row-by-row running total.
* **Why it breaks:** When evaluating a window function at Step 6, adding an `ORDER BY` tells the query processor to treat the framing bounds as `RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW` (a running tally). If you omit the sorting clause, the engine assumes the frame is the entire partition chunk (`RANGE BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING`).
* **The Result:** Instead of an incremental step calculation, every row in the partition receives the exact same final aggregated sum.
```sql
-- 🚫 FAILS (Returns the total sum of all rows on every single line)
SELECT TransactionId, Amount,
       SUM(Amount) OVER (PARTITION BY AccountId) as RunningTotal
FROM Ledger;

-- ✅ FIXED (Forces the execution planner to accumulate sequentially)
SELECT TransactionId, Amount,
       SUM(Amount) OVER (PARTITION BY AccountId ORDER BY TransactionDate) as RunningTotal
FROM Ledger;
```

---

## 💡 Pro-Tips for the LexisNexis Interview Panel
1. **Never use deep subquery nesting.** Opt for clear, modular **Common Table Expressions (CTEs)**. This signals to engineering leads that you prioritize code readability and maintainability.
2. **Mention memory buffers (TempDB).** When discussing `GROUP BY` or `DISTINCT`, mention that filtering out records early using `WHERE` minimizes data volume before it hits the database sorting buffers.
