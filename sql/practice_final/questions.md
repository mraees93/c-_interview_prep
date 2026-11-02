Schema details:
Departments (`DeptID`, `DeptName`, `Region`)
Lawyers (`LawyerID`, `FullName`, `DeptID`, `BillingRateZAR`)
LegalDocuments (`DocID`, `Title`, `DocType`, `CreatedDate`, `AuthorID`)
DocumentAccessLogs (`LogID`, `DocID`, `UserRole`, `Action`, `DurationSeconds`, `AccessTimestamp`)

1. Write a query to list all DocID and Title values from LegalDocuments, along with the UserRole from DocumentAccessLogs. The output must include all documents regardless of whether they have an entry in the access logs.

2. Write a query to find the FullName of all lawyers who have never authored a document in the database. (Join Lawyers to LegalDocuments and filter out the matched rows using IS NULL).

3. Write a query displaying the DeptID and the average BillingRateZAR of the lawyers inside each department. Only include departments where the average billing rate is strictly greater than 1500 ZAR.

4. Select the DocID, Title, DocType, and CreatedDate from LegalDocuments. Add a window function column named DocumentSequence that assigns a sequential row number to every document starting from 1, ordered from the newest creation date to the oldest.


Question 5 (Dense Ranking)

Show the FullName, DeptID, and BillingRateZAR for all lawyers. Include a window function column named RateRank that ranks lawyers inside their specific DeptID based on their BillingRateZAR from highest to lowest. Ensure that tied rates share the same rank and that the numbering sequence does not skip numbers after a tie.

Question 6 (Inline Multi-Table Aggregation)

Join Lawyers and Departments. Select the FullName, DeptName, and BillingRateZAR, alongside an inline window function column named RegionalAverage showing the average billing rate of all lawyers within that lawyer's specific geographic Region. Do not use a structural GROUP BY clause.

Question 7 (Partitioned Subset Tracking)

Query the DocumentAccessLogs table. Extract the LogID, DocID, UserRole, and AccessTimestamp, along with a sequential row number named AccessSequence applied across each individual DocID subset, ordered from the newest access attempt to the oldest.

Question 8 (Inline Cumulative Running Sum)

For each access entry in DocumentAccessLogs, return the LogID, DocID, DurationSeconds, and AccessTimestamp, along with a column named RunningTotalDuration showing the cumulative running total duration spent reviewing that specific DocID over time (ordered chronologically by AccessTimestamp).

Question 9 (Filtered Window Subset)

Write a query to find the DocID, Title, DocType, and CreatedDate of only the documents that are the single absolute oldest document recorded within each specific DocType. Use a CTE with a window function to filter the results.

Question 10 (Sargable Optimization Code)

Write an optimized, sargable query to select all columns from DocumentAccessLogs where the AccessTimestamp occurred at any point during the calendar year 2026. Do not use date-part extraction functions on the column.