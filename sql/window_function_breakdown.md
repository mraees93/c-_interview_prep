# SQL Window Function Execution Visualizer
This document breaks down how an intermediate analytical SQL query runs step-by-step using visual data tables.

## 📋 The Database Schema Reference
*   **Lawyers** (`LawyerID`, `Name`, `Department`)
*   **Matters** (`MatterID`, `Title`, `LeadLawyerID`)
*   **Documents** (`DocID`, `MatterID`, `FileSizeKB`)

---

## 🛠️ The Target Query
```sql
SELECT Department, DocID, FileSizeKB
FROM (
    SELECT l.Department, d.DocID, d.FileSizeKB,
           ROW_NUMBER() OVER(
               PARTITION BY l.Department 
               ORDER BY d.FileSizeKB DESC
           ) AS rnk
    FROM Lawyers l
    JOIN Matters m ON l.LawyerID = m.LeadLawyerID
    JOIN Documents d ON m.MatterID = d.MatterID
) t
WHERE rnk = 1;
```

---

## 🔄 Step-by-Step Data Flow Execution

### Phase 1: Raw Relational Joins
The database maps the foreign keys across `Lawyers`, `Matters`, and `Documents` to create a flat, unstructured dataset in memory.

| Department | DocID | FileSizeKB |
| :--- | :--- | :--- |
| Litigation | Doc-A | 5000 |
| Litigation | Doc-B | 15000 |
| Litigation | Doc-C | 2000 |
| Corporate | Doc-D | 8000 |
| Corporate | Doc-E | 22000 |

### Phase 2: Evaluating the Window Function (`OVER()`)
The engine groups the dataset into isolated virtual category windows via `PARTITION BY`, sorts the contents inside those walls via `ORDER BY ... DESC`, and stamps a sequential row number (`rnk`) to form the subquery table expression **`t`**.

#### 📂 Window Partition: Litigation

| Department | DocID | FileSizeKB | rnk |
| :--- | :--- | :--- | :--- |
| **Litigation** | **Doc-B** | **15000** | **1** *(Largest)* |
| Litigation | Doc-A | 5000 | 2 |
| Litigation | Doc-C | 2000 | 3 |

#### 📂 Window Partition: Corporate

| Department | DocID | FileSizeKB | rnk |
| :--- | :--- | :--- | :--- |
| **Corporate** | **Doc-E** | **22000** | **1** *(Largest)* |
| Corporate | Doc-D | 8000 | 2 |

### Phase 3: The Outer Filter Execution (`WHERE rnk = 1`)
The outer query evaluates the compiled subquery table dataset `t` against the conditional filter statement, discarding any non-matching rows.

| Department | DocID | FileSizeKB | rnk | Action |
| :--- | :--- | :--- | :--- | :--- |
| Litigation | Doc-B | 15000 | 1 | ✅ **KEEP** |
| Litigation | Doc-A | 5000 | 2 | ❌ *DROP* |
| Litigation | Doc-C | 2000 | 3 | ❌ *DROP* |
| Corporate | Doc-E | 22000 | 1 | ✅ **KEEP** |
| Corporate | Doc-D | 8000 | 2 | ❌ *DROP* |

### Phase 4: Final Column Target Projection
The top-level `SELECT` list isolates the specific columns requested by the user application interface, safely dropping the helper metadata column (`rnk`) from the output matrix.

| Department | DocID | FileSizeKB |
| :--- | :--- | :--- |
| Litigation | Doc-B | 15000 |
| Corporate | Doc-E | 22000 |
