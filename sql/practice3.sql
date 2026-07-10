
--2. List the FullName of the candidate, the CheckType, and the Status for all individual checks that have been 'Flagged'.

SELECT c.FullName, v.CheckType, v.Status
FROM Candidates c
JOIN Verifications v ON c.CandidateID = v.CandidateID
WHERE v.Status = 'Flagged';

-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--4. Display the Industry name and the total count of candidates submitted under each industry.

SELECT cl.Industry, COUNT(ca.CandidateID) AS TotalCount
FROM Clients cl
LEFT JOIN Candidates ca ON cl.ClientID = ca.ClientID --If an industry exists in the Clients table but has zero candidates submitted yet
GROUP BY cl.Industry;
