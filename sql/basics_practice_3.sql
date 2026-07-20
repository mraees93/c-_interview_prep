--9. Write a query to show the CandidateID, CheckType, and CostZAR, alongside a new column displaying the average cost of that specific CheckType across the entire 
-- database.

SELECT CandidateID, CheckType, CostZAR,
    AVG(CostZAR) OVER(PARTITION BY CheckType) AS AvgCostOfCheckType
FROM Verifications;

-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--10. Select the CandidateID, FullName, and SubmissionDate, along with a column that assigns a sequential row number to candidates for each unique client, 
-- ordered by their SubmissionDate from earliest to latest.

SELECT CandidateID, FullName, SubmissionDate,
        ROW_NUMBER() OVER(PARTITION BY ClientID ORDER BY SubmissionDate ASC) AS CandidatesBySubmissionDate
FROM Candidates;

SELECT CandidateID, FullName, SubmissionDate,
    ROW_NUMBER() OVER(PARTITION BY ClientID ORDER BY SubmissionDate ASC) AS CandidatesBySubmissionDate
FROM Candidates;