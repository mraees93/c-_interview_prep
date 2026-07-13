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

-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--5. List all corporate CompanyName entries that currently have zero candidates submitted in the Candidates table.

