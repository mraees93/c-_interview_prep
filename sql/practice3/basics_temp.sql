-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

-- 13. For each client, find the single candidate who has the highest total combined verification CostZAR. 
-- Return the CompanyName, FullName, and the total combined cost. If there is a tie, return only one row per client.

WITH CandidateTotals AS (
    SELECT cl.ClientID, ca.CandidateID, SUM(v.CostZAR) AS TotalCost
    FROM Clients cl
    JOIN Candidates ca ON cl.ClientID = ca.ClientID
    JOIN Verifications v ON ca.CandidateID = v.CandidateID
    GROUP BY cl.ClientID, ca.CandidateID
),
RankedTotals AS (
    SELECT CompanyName, FullName, TotalCost,
           RANK() OVER(PARTITION BY ClientID ORDER BY TotalCost DESC) AS HighestTotal
    FROM CandidateTotals
)
SELECT CompanyName, FullName, TotalCost
FROM RankedTotals
WHERE HighestTotal = 1;