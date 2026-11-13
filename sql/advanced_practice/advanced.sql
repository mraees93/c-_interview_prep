-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--9. Write a query to show the CandidateID, CheckType, and CostZAR, alongside a new column displaying the average cost of that specific CheckType across the entire 
-- database.

SELECT CandidateID, CheckType, CostZAR,
      AVG(CostZAR) OVER(PARTITION BY CheckType) AS AvgCostOfCheckType
FROM Verifications;

--10. Select the CandidateID, FullName, and SubmissionDate, along with a column that assigns a sequential row number to candidates for each unique client, 
-- ordered by their SubmissionDate from earliest to latest.

SELECT CandidateID, FullName, SubmissionDate,
      ROW_NUMBER() OVER(PARTITION BY ClientID ORDER BY SubmissionDate ASC) AS CandidatesBySubmissionDate
FROM Candidates;

SELECT CandidateID, FullName, SubmissionDate,
    ROW_NUMBER() OVER(PARTITION BY ClientID ORDER BY SubmissionDate ASC) AS CandidatesBySubmissionDate
FROM Candidates;


-- 12. Show the CheckID, CandidateID, CostZAR, and a dense ranking of the check costs within each specific CheckType, where the most 
-- expensive check gets a rank of 1.

WITH RankingCheckTypes AS (
    SELECT CheckID, CandidateID, CostZAR,
           DENSE_RANK() OVER(PARTITION BY CheckType ORDER BY CostZAR DESC) AS CheckTypeRank
    FROM Verifications
)
SELECT CheckID, CandidateID, CostZAR
FROM RankingCheckTypes
WHERE CheckTypeRank = 1;