-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 1. List all Lawyers and their total file storage. If a lawyer has no files, show 0 instead of NULL

SELECT l.Name, COALESCE(SUM(d.FileSizeKB), 0) AS TotalFileStorage
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.LawyerID, l.Name;

-- 2. Find the Names of Lawyers who have at least one Matter, but that Matter has zero documents.

SELECT DISTINCT l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
WHERE d.DocID IS NULL;


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 3. Calculate total documents per Lawyer, ensuring that if two lawyers have the same name, they are not combined.

SELECT l.Name, COUNT(d.DocID) AS DocumentCount
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.LawyerID, l.Name;

-- 1. List all Lawyers and their total file storage. If a lawyer has no files, show 0 instead of NULL

SELECT l.Name, COALESCE(SUM(d.FileSizeKB), 0) AS TotalFileStorage
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.LawyerID, l.Name;


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 2. Find the Names of Lawyers who have at least one Matter, but that Matter has zero documents.

SELECT l.Name, COUNT(m.MatterID) AS MatterCount
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.LawyerID, l.Name, d.DocID
HAVING d.DocID IS NULL;

--answer:
SELECT DISTINCT l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
WHERE d.DocID IS NULL;

-- Key Concept: Combining an INNER JOIN (must have a matter) with a LEFT JOIN (to check for missing documents).
-- The reason we use LEFT JOIN for the documents is specifically so we don't lose the Matter title just because it's empty. 
-- If you used an INNER JOIN there, any matter without a document would vanish from the list entirely!