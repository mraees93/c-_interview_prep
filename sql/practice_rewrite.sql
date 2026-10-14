-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

-- 13. For each client, find the single candidate who has the highest total combined verification CostZAR. 
-- Return the CompanyName, FullName, and the total combined cost. If there is a tie, return only one row per client.

WITH CandidateTotals AS (
    SELECT cl.ClientID, cl.CompanyName, ca.CandidateID, ca.FullName, SUM(v.CostZAR) AS TotalCost
    FROM Clients cl
    JOIN Candidates ca ON cl.ClientID = ca.ClientID
    JOIN Verifications v ON ca.CandidateID = v.CandidateID
    GROUP BY cl.ClientID, cl.CompanyName, ca.FullName
),
RankedCandidates AS (
    SELECT CompanyName, FullName, TotalCost,
           ROW_NUMBER() OVER(PARTITION BY ClientID ORDER BY TotalCost DESC) AS rank
    FROM CandidateTotals
)
SELECT CompanyName, FullName, TotalCost
FROM RankedCandidates 
WHERE rank = 1;


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