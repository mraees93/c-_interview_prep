# MS SQL Window Functions Cheat Sheet

## 🪟 Concept definitions
*   **Window Function**: Calculates aggregate or positional metrics across a set of table rows related to the current row without collapsing the rows into a single summary output (unlike a standard GROUP BY). Every row retains its unique identity.
*   **OVER Clause**: Defines the specific subset of rows (the "window") the function should look at and determines the sequence of execution for that calculation.

## 💡 Core Syntax
```sql
SELECT col,
       WINDOW_FUNC() OVER (
           PARTITION BY group_col
           ORDER BY sort_col ASC|DESC
           ROWS|RANGE BETWEEN lower_bound AND upper_bound
       ) AS alias
FROM table;
```

## 🧱 OVER Clause Components
*   `PARTITION BY`: Divides the dataset into groups or "chunks" (Optional. If omitted, the entire table is processed as one single window).
*   `ORDER BY`: Establishes the physical processing sequence of rows *inside* each partition (Required for value and ranking functions).
*   `ROWS / RANGE`: Defines a sliding frame of rows relative to the current row (e.g., `ROWS BETWEEN 2 PRECEDING AND CURRENT ROW`).

## 🧩 When to use PARTITION BY?
Ask: *"Do I want my calculation to reset when a specific category changes?"*

*   ❌ **Omit it (Global)**: If you want a continuous calculation across the entire table (e.g., a master leaderboard or company-wide lifetime running total).
*   ✅ **Include it (Grouped)**: If you want the calculation to isolate itself to specific groups and **reset back to 0 or 1** when a new group starts (e.g., finding the top 3 products *per category*, or calculating running totals *per customer*).

### Quick Contrast
*   **Top 3 highest-paid employees globally**: `ORDER BY salary DESC`
*   **Top 3 highest-paid employees *per department***: `PARTITION BY department_id ORDER BY salary DESC`

## ⚔️ MAX() OVER() vs. Ranking Functions
Ask: *"Do I want to compare all rows against the peak value, or do I want to filter down to just the top row?"*

*   🔍 **Use `MAX() OVER()` (Comparison)**: Appends the highest value to every single row in the group without hiding anything.
    ```sql
    -- Shows everyone, plus their department's maximum salary for comparison
    SELECT employee_name, department_id, salary,
           MAX(salary) OVER(PARTITION BY department_id) as max_dept_salary
    FROM employees;
    ```
*   🎯 **Use `ROW_NUMBER() OVER()` (Filtering)**: Numbers rows sequentially so you can discard the rest and extract just the top record.
    ```sql
    -- Used with a CTE to filter out everything except the #1 spot
    WITH Ranked AS (
        SELECT employee_name, department_id, salary,
               ROW_NUMBER() OVER(PARTITION BY department_id ORDER BY salary DESC) as rn
        FROM employees
    )
    SELECT employee_name, department_id, salary FROM Ranked WHERE rn = 1;
    ```

## 📌 Ranking Functions
*   `ROW_NUMBER()`: Assigns a unique sequential integer starting at 1 to each row inside the partition.
*   `RANK()`: Assigns a shared rank for ties; skips subsequent sequence numbers to account for ties (e.g., 1, 1, 3).
*   `DENSE_RANK()`: Assigns a shared rank for ties; retains consecutive, gapless sequence numbers (e.g., 1, 1, 2).
*   `NTILE(N)`: Divides the rows within a partition into `N` approximately equal buckets and assigns the bucket number (e.g., quartiles = 4).

## 🔎 Value Functions
*   `LAG(col, offset)`: Fetches data from a specified physical row *before* the current row inside the partition.
*   `LEAD(col, offset)`: Fetches data from a specified physical row *after* the current row inside the partition.
*   `FIRST_VALUE(col)`: Fetches the very first value from the designated window frame.
*   `LAST_VALUE(col)`: Fetches the very last value from the designated window frame.
*   *Note: `NTH_VALUE` is NOT supported in MS SQL. Use a CTE with `ROW_NUMBER() = N` instead.*

## 📊 Running Aggregates
*   `SUM(col) OVER(...)`: Calculates a running or moving total across the window.
*   `AVG(col) OVER(...)`: Calculates a rolling or moving average across the window.
*   `COUNT(col) OVER(...)`: Calculates a dynamic cumulative tally of records across the window.
*   `MIN(col) / MAX(col) OVER(...)`: Tracks the running minimum or maximum value across the window.

## 🎛️ Frame Bounds (ROWS / RANGE)
*   `UNBOUNDED PRECEDING`: Starts evaluating from the very first row of the partition.
*   `CURRENT ROW`: Stops or starts evaluation at the row currently being processed.
*   `UNBOUNDED FOLLOWING`: Extends evaluation to the very last row of the partition.
*   `N PRECEDING / FOLLOWING`: Limits evaluation to exactly `N` rows before or after the current row.

## ⚠️ Essential Rules
*   **No WHERE filtering**: Window functions execute after `WHERE`. Use a **CTE** or subquery to filter on window outputs.
*   **The LAST_VALUE Trap**: Default sorting frame is `RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW`. For absolute last values, force: `ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING`.
*   **WINDOW Clause**: Available in **SQL Server 2022+** to reuse frame definitions (`WINDOW w AS (PARTITION BY x ORDER BY y)`).

## ⚡ Quick Snippets

### Deduplicate Records
```sql
WITH CTE AS (
    SELECT id, ROW_NUMBER() OVER(PARTITION BY unique_col ORDER BY date_col DESC) as rn
    FROM my_table
)
DELETE FROM CTE WHERE rn > 1;
```

### Running Total & 7-Day Average
```sql
SELECT date, sales,
       SUM(sales) OVER(ORDER BY date ROWS UNBOUNDED PRECEDING) as running_total,
       AVG(sales) OVER(ORDER BY date ROWS BETWEEN 6 PRECEDING AND CURRENT ROW) as moving_avg_7d
FROM daily_sales;
```

### Month-over-Month Change
```sql
SELECT month, revenue,
       LAG(revenue, 1) OVER(ORDER BY month) as prev_month,
       revenue - LAG(revenue, 1) OVER(ORDER BY month) as mom_diff
FROM revenue_table;
```
