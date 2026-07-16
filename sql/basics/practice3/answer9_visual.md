# 🪟 Visual Breakdown: Aggregating Averages using Window Functions

-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--9. Write a query to show the CandidateID, CheckType, and CostZAR, alongside a new column displaying the average cost of that specific CheckType across the entire 
-- database.

```sql
SELECT CandidateID, CheckType, CostZAR, 
       AVG(CostZAR) OVER(PARTITION BY CheckType) AS AvgCostCheckType
FROM Verifications;
```

---

### Step 1: The Raw Data Pool (`FROM Verifications`)
The database engine pulls all records from the `Verifications` table. Notice the check types ('Criminal' and 'Credit') and their related costs are completely mixed up.

| CheckID | CandidateID | CheckType | CostZAR | Status |
| :--- | :--- | :--- | :--- | :--- |
| **101** | 1 | Criminal | 400.00 | Complete |
| **102** | 1 | Credit | 150.00 | Pending |
| **103** | 2 | Criminal | 500.00 | Complete |
| **104** | 3 | Credit | 250.00 | Complete |
| **105** | 4 | Criminal | 300.00 | Flagged |

---

### Step 2: The `PARTITION BY CheckType` (Grouping for the Math)
The window function isolates rows into independent memory buckets based on the `CheckType`. This allows the engine to calculate individual math metrics for each type without compressing or merging the rows.

#### 📦 Bucket A: CheckType = Credit

| CandidateID | CheckType | CostZAR |
| :--- | :--- | :--- |
| 1 | Credit | 150.00 |
| 3 | Credit | 250.00 |

#### 📦 Bucket B: CheckType = Criminal

| CandidateID | CheckType | CostZAR |
| :--- | :--- | :--- |
| 1 | Criminal | 400.00 |
| 2 | Criminal | 500.00 |
| 4 | Criminal | 300.00 |

---

### Step 3: The Window Aggregation Calculation
The engine calculates the `AVG(CostZAR)` *inside each bucket separately*. 

* **Credit Average:** `(150 + 250) / 2 =` **`200.00`**
* **Criminal Average:** `(400 + 500 + 300) / 3 =` **`400.00`**

It then assigns this fixed baseline calculation back to **every single row** inside its respective bucket.

#### 📦 Bucket A: CheckType = Credit (Calculated)

| CandidateID | CheckType | CostZAR | AvgCostCheckType |
| :--- | :--- | :--- | :--- |
| 1 | Credit | 150.00 | **200.00** |
| 3 | Credit | 250.00 | **200.00** |

#### 📦 Bucket B: CheckType = Criminal (Calculated)

| CandidateID | CheckType | CostZAR | AvgCostCheckType |
| :--- | :--- | :--- | :--- |
| 1 | Criminal | 400.00 | **400.00** |
| 2 | Criminal | 500.00 | **400.00** |
| 4 | Criminal | 300.00 | **400.00** |

---

### Final Output (`SELECT`)
The engine displays the selected columns. Unlike a standard `GROUP BY` clause which would squash your data down into just 2 summary rows, the window function retains your individual raw data rows (`CandidateID`, `CostZAR`) while appending the calculated category average cleanly to each one.

| CandidateID | CheckType | CostZAR | AvgCostCheckType |
| :--- | :--- | :--- | :--- |
| **1** | Credit | 150.00 | **200.00** |
| **3** | Credit | 250.00 | **200.00** |
| **1** | Criminal | 400.00 | **400.00** |
| **2** | Criminal | 500.00 | **400.00** |
| **4** | Criminal | 300.00 | **400.00** |
