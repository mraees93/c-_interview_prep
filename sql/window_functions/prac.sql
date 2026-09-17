-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 7. Find the single largest document (highest FileSizeKB) for each Department. Show the Department name, the Document ID, and the size.

WITH RankedDocuments AS (
    SELECT l.Department, d.DocID, d.FileSizeKB,
           ROW_NUMBER() OVER(PARTITION BY l.Department ORDER BY d.FileSizeKB DESC) AS rank
    FROM Lawyers l
    JOIN Matters m ON l.LawyerID = m.LeadLawyerID
    JOIN Documents d ON m.MatterID = d.MatterID
)
SELECT Department, DocID, FileSizeKB
FROM RankedDocuments
WHERE rank = 1;