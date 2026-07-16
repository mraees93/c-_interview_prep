# 🪟 Visual Breakdown: Partitioned Row Numbers

-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--10. Select the CandidateID, FullName, and SubmissionDate, along with a column that assigns a sequential row number to candidates for each unique client, ordered by their SubmissionDate from earliest to latest.

```sql
SELECT CandidateID, FullName, SubmissionDate,
    ROW_NUMBER() OVER(PARTITION BY ClientID ORDER BY SubmissionDate ASC) AS CandidatesBySubmissionDate
FROM Candidates;
```

---

### Step 1: The Raw Data Pool (`FROM Candidates`)
The engine grabs the raw data from the `Candidates` table. Notice we have candidates belonging to two different clients (Client IDs **10** and **20**), and their submission dates are completely unorganized.

| CandidateID | ClientID | FullName | SubmissionDate |
| :--- | :--- | :--- | :--- |
| **1** | 10 | John Doe | 2026-02-15 |
| **2** | 20 | Jane Smith | 2026-01-10 |
| **3** | 10 | Alice Brown | 2026-01-05 |
| **4** | 20 | Bob Johnson | 2026-03-22 |
| **5** | 10 | Charlie Green | 2026-02-01 |

---

### Step 2: The `PARTITION BY ClientID` (Slicing the Data)
The window function isolates rows into separate independent buckets based on the `ClientID`. Operations inside one bucket will never affect or interact with the rows in another bucket.

#### 📦 Bucket A: ClientID = 10

| CandidateID | ClientID | FullName | SubmissionDate |
| :--- | :--- | :--- | :--- |
| **1** | 10 | John Doe | 2026-02-15 |
| **3** | 10 | Alice Brown | 2026-01-05 |
| **5** | 10 | Charlie Green | 2026-02-01 |

#### 📦 Bucket B: ClientID = 20

| CandidateID | ClientID | FullName | SubmissionDate |
| :--- | :--- | :--- | :--- |
| **2** | 20 | Jane Smith | 2026-01-10 |
| **4** | 20 | Bob Johnson | 2026-03-22 |

---

### Step 3: The `ORDER BY SubmissionDate ASC` (Sorting inside the Buckets)
The engine reorders the rows *inside each bucket individually* from the earliest date to the latest date. 

#### 📦 Bucket A: ClientID = 10 (Sorted)

| CandidateID | ClientID | FullName | SubmissionDate |
| :--- | :--- | :--- | :--- |
| **3** | 10 | Alice Brown | **2026-01-05** *(Earliest)* |
| **5** | 10 | Charlie Green | **2026-02-01** |
| **1** | 10 | John Doe | **2026-02-15** *(Latest)* |

#### 📦 Bucket B: ClientID = 20 (Sorted)

| CandidateID | ClientID | FullName | SubmissionDate |
| :--- | :--- | :--- | :--- |
| **2** | 20 | Jane Smith | **2026-01-10** *(Earliest)* |
| **4** | 20 | Bob Johnson | **2026-03-22** *(Latest)* |

---

### Step 4: The `ROW_NUMBER()` Assignment
The engine assigns sequential numbers starting at **1** for each row. The counter **resets** completely back to 1 when crossing over into a new bucket.

#### 📦 Bucket A: ClientID = 10

| CandidateID | FullName | SubmissionDate | CandidatesBySubmissionDate |
| :--- | :--- | :--- | :--- |
| **3** | Alice Brown | 2026-01-05 | **1** |
| **5** | Charlie Green | 2026-02-01 | **2** |
| **1** | John Doe | 2026-02-15 | **3** |

#### 📦 Bucket B: ClientID = 20 (Counter Resets)

| CandidateID | FullName | SubmissionDate | CandidatesBySubmissionDate |
| :--- | :--- | :--- | :--- |
| **2** | Jane Smith | 2026-01-10 | **1** |
| **4** | Bob Johnson | 2026-03-22 | **2** |

---

### Final Output (`SELECT`)
The final view hides the background `ClientID` tracking column because it wasn't explicitly requested in your outer column display list, presenting the records cleanly arranged by their buckets.

| CandidateID | FullName | SubmissionDate | CandidatesBySubmissionDate |
| :--- | :--- | :--- | :--- |
| **3** | Alice Brown | 2026-01-05 | **1** |
| **5** | Charlie Green | 2026-02-01 | **2** |
| **1** | John Doe | 2026-02-15 | **3** |
| **2** | Jane Smith | 2026-01-10 | **1** |
| **4** | Bob Johnson | 2026-03-22 | **2** |
