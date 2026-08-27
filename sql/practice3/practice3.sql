--1. Find the FullName and SubmissionDate of all candidates who submitted their details in January 2026.

SELECT FullName, SubmissionDate
FROM Candidates
WHERE SubmissionDate >= '2026-01-01'
    AND SubmissionDate < '2026-02-01';


--2. List the FullName of the candidate, the CheckType, and the Status for all individual checks that have been 'Flagged'.

SELECT c.FullName, v.CheckType, v.Status
FROM Candidates c
JOIN Verifications v ON c.CandidateID = v.CandidateID
WHERE v.Status = 'Flagged';

--3. Find the CompanyName and the total sum of CostZAR spent on all verifications ordered by that client.

SELECT c.CompanyName, SUM(v.CostZAR) AS TotalVerificationsOrdered
FROM Clients c
JOIN Candidates ca ON c.ClientID = ca.ClientID
JOIN Verifications v ON ca.CandidateID = v.CandidateID
GROUP BY c.CompanyName;


--4. Display the Industry name and the total count of candidates submitted under each industry.

SELECT cl.Industry, COUNT(ca.CandidateID) AS TotalCount
FROM Clients cl
LEFT JOIN Candidates ca ON cl.ClientID = ca.ClientID --If an industry exists in the Clients table but has zero candidates submitted yet
GROUP BY cl.Industry;


--5. List all corporate CompanyName entries that currently have zero candidates submitted in the Candidates table.

SELECT c.CompanyName
FROM Clients c
LEFT JOIN Candidates ca ON c.ClientID = ca.ClientID
WHERE ca.CandidateID IS NULL;

--alternate:
SELECT c.CompanyName
FROM Clients c
WHERE NOT EXISTS (
    SELECT 1 
    FROM Candidates ca 
    WHERE ca.ClientID = c.ClientID
);
/*
The SELECT 1: The subquery does not need to waste resources fetching actual data rows (like ca.CandidateID). It just returns a 1 (true) the second it finds a match, 
making it highly efficient.Correlated Filter: The WHERE ca.ClientID = c.ClientID links the inner query to the outer query, evaluating each company one by one.
*/


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
JOIN Verifications p ON c.CandidateID = p.CandidateID AND p.Status = 'Pending' -- filters candidates to only include individuals who have at least one 'Pending' check
LEFT JOIN Verifications f ON c.CandidateID = f.CandidateID AND f.Status = 'Flagged'--get Flagged for the candidates. If candidate has no 'flagged', f columns become null
WHERE f.CandidateID IS NULL; --drop anyone who has a flagged status

--7. Find the CompanyName of clients whose average verification check cost (CostZAR) across all their candidates is strictly greater than 200.00.

SELECT cl.CompanyName, AVG(v.CostZAR) AS AverageVerificationCost
FROM Clients cl
JOIN Candidates ca ON cl.ClientID = ca.ClientID
JOIN Verifications v ON ca.CandidateID = v.CandidateID
GROUP BY cl.CompanyName 
HAVING AVG(v.CostZAR) > 200;

--8. List every CheckID and its CheckType, alongside a count of how many total log entries exist for that check in the VerificationLogs table. Include checks even if 
-- they have zero logs.

SELECT v.CheckID, v.CheckType, COUNT(vl.CheckID) AS TotalLogEntries
FROM Verifications v
LEFT JOIN VerificationLogs vl ON v.CheckID = vl.CheckID
GROUP BY v.CheckID, v.CheckType;

--9. Write a query to show the CandidateID, CheckType, and CostZAR, alongside a new column displaying the average cost of that specific CheckType across the entire 
-- database.

SELECT CandidateID, CheckType, CostZAR, 
       AVG(CostZAR) OVER(PARTITION BY CheckType) AS AvgCostCheckType -- Groups the average calculation by CheckType across the whole table
FROM Verifications;

--10. Select the CandidateID, FullName, and SubmissionDate, along with a column that assigns a sequential row number to candidates for each unique client, 
-- ordered by their SubmissionDate from earliest to latest.

SELECT CandidateID, FullName, SubmissionDate,
    ROW_NUMBER() OVER(PARTITION BY ClientID ORDER BY SubmissionDate ASC) AS CandidatesBySubmissionDate
FROM Candidates;

-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--11. Find the CandidateID, CheckType, and Status for all verifications where the check's CostZAR is higher than the average CostZAR of all checks belonging to 
-- that specific candidate's client.

--Sub query version:
SELECT CandidateID, CheckType, Status
FROM (
    SELECT c.ClientID, v.CandidateID, v.CheckType, v.CostZAR, v.Status,
           AVG(v.CostZAR) OVER(PARTITION BY c.ClientID) AS AverageCostZAR
    FROM Candidates c
    JOIN Verifications v ON c.CandidateID = v.CandidateID
) t
WHERE CostZAR > AverageCostZAR;

-- Common table expression(CTE) version

WITH HighestCostZAR AS (
    SELECT c.ClientID, v.CandidateID, v.CheckType, v.CostZAR, v.Status,
           AVG(v.CostZAR) OVER(PARTITION BY c.ClientID) AS AverageCostZAR
    FROM Candidates c
    JOIN Verifications v ON c.CandidateID = v.CandidateID
)
SELECT CandidateID, CheckType, Status
FROM HighestCostZAR
WHERE CostZAR > AverageCostZAR;

--"I chose a CTE to separate the concerns of the calculation logic from the final filtering logic, maximizing code readability and maintainability for the team.


-- 12. Show the CheckID, CandidateID, CostZAR, and a dense ranking of the check costs within each specific CheckType, where the most expensive check gets a rank of 1.

SELECT CheckID, CandidateID, CostZAR,
        DENSE_RANK() OVER(PARTITION BY CheckType ORDER BY CostZAR DESC) AS RankingTheCheckCosts
FROM Verifications;


-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

-- 13. For each client, find the single candidate who has the highest total combined verification CostZAR. 
-- Return the CompanyName, FullName, and the total combined cost. If there is a tie, return only one row per client.

WITH HighestTotal AS (
    SELECT c.ClientID, v.CandidateID, SUM(v.CostZAR),
            ROW_NUMBER() OVER(PARTITION BY c.ClientID ORDER BY SUM(v.CostZAR) DESC) AS rank
    FROM Candidates c 
    JOIN Verifications v ON c.CandidateID = v.CandidateID
    GROUP BY c.ClientID, v.CandidateID
)
SELECT CandidateID 
FROM HighestTotal 
WHERE rank = 1;

-- TODO: Add 2 CTE'S for production - best practice

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
