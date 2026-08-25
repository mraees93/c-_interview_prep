# 🪟 Visual Breakdown: Multi-Layer Aggregation & Ranking

### The Problem
For each client, find the single candidate who has the highest total combined verification `CostZAR`. Return the `CompanyName`, `FullName`, and the total combined cost. If there is a tie, return only one row per client.

-- Clients (ClientID, CompanyName, Industry)<br>
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)<br>
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)<br>
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)<br>

---

### The Query
```sql
WITH CandidateTotals AS (
    -- Step 1: Safely sum up the total costs per candidate
    SELECT 
        cl.CompanyName,
        ca.FullName,
        ca.ClientID,
        SUM(v.CostZAR) AS TotalCost
    FROM Clients cl
    JOIN Candidates ca ON cl.ClientID = ca.ClientID
    JOIN Verifications v ON ca.CandidateID = v.CandidateID
    GROUP BY cl.CompanyName, ca.FullName, ca.ClientID
),
RankedCandidates AS (
    -- Step 2: Rank the candidates within each client group
    SELECT 
        CompanyName,
        FullName,
        TotalCost,
        ROW_NUMBER() OVER(
            PARTITION BY ClientID 
            ORDER BY TotalCost DESC
        ) AS rnk
    FROM CandidateTotals
)
-- Step 3: Extract only the top-spending candidate per client
SELECT CompanyName, FullName, TotalCost
FROM RankedCandidates
WHERE rnk = 1;
```

---

### 💡 Core Interview Concepts

* **The Multi-Step CTE Strategy:** Trying to calculate a `SUM()` and a `ROW_NUMBER()` in the exact same query block is illegal in SQL because you cannot nest window functions inside aggregations. Splitting the logic into two sequential CTE layers (`CandidateTotals` then `RankedCandidates`) keeps the execution plan highly structured and readable.
* **Tie Breaker Mechanics:** The prompt asks for *only one row per client* if a tie occurs. Using `ROW_NUMBER()` fulfills this constraint perfectly because it forces a sequential sequence (1, 2, 3), ensuring only one record ever passes the final `WHERE rnk = 1` boundary. (If you used `DENSE_RANK()`, a tie would return multiple candidates for that client).

---

### Step 1: Layer 1 Output (`CandidateTotals`)
The engine joins all three tables, sums up the verification costs, and groups them down to one single row per candidate profile.

| CompanyName | FullName | ClientID | TotalCost |
| :--- | :--- | :--- | :--- |
| **Acme Corp** | John Doe | 10 | 550.00 |
| **Acme Corp** | Alice Brown | 10 | 350.00 |
| **TechStart** | Jane Smith | 20 | 600.00 |
| **TechStart** | Bob Johnson | 20 | 600.00 *(Tie)* |

---

### Step 2: Layer 2 Output (`RankedCandidates`)
The engine reads from the first layer, sections the rows into client buckets using `PARTITION BY ClientID`, and sorts them from highest cost to lowest. 

`ROW_NUMBER()` forces a strict sequential rank assignment (1, 2, 3), meaning it will arbitrarily select only one winner in the event of a tie (like TechStart).

#### 📦 Client Bucket 10 (Acme Corp)

| CompanyName | FullName | TotalCost | rnk |
| :--- | :--- | :--- | :--- |
| Acme Corp | John Doe | 550.00 | **1** |
| Acme Corp | Alice Brown | 350.00 | **2** |

#### 📦 Client Bucket 20 (TechStart)

| CompanyName | FullName | TotalCost | rnk |
| :--- | :--- | :--- | :--- |
| TechStart | Jane Smith | 600.00 | **1** *(Arbitrary Winner)* |
| TechStart | Bob Johnson | 600.00 | **2** *(Dropped due to strict sequence)* |

---

### Step 3: Final Output Filter (`WHERE rnk = 1`)
The outer query evaluates the compiled rows and completely strips away anything that isn't rank #1.

| CompanyName | FullName | TotalCost |
| :--- | :--- | :--- |
| **Acme Corp** | John Doe | 550.00 |
| **TechStart** | Jane Smith | 600.00 |
