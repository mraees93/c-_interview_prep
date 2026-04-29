-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 1. List all Lawyers and their total file storage. If a lawyer has no files, show 0 instead of NULL

SELECT l.Name, COALESCE(SUM(d.FileSizeKB), 0) AS TotalFileStorage
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.Name;


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 2. Find the Names of Lawyers who have at least one Matter, but that Matter has zero documents.

--try:
SELECT l.Name, m.Title, COUNT(d.MatterID) AS DocumentCount
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
WHERE COUNT(d.MatterID) >= 1
GROUP BY l.Name;

--answer:
SELECT DISTINCT l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
WHERE d.DocID IS NULL;

-- Key Concept: Combining an INNER JOIN (must have a matter) with a LEFT JOIN (to check for missing documents).