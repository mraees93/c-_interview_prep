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
-- The reason we use LEFT JOIN for the documents is specifically so we don't lose the Matter title just because it's empty. 
-- If you used an INNER JOIN there, any matter without a document would vanish from the list entirely!


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 3. Calculate total documents per Lawyer, ensuring that if two lawyers have the same name, they are not combined.

--try:
SELECT l.LawyerID, l.Name, SUM(d.DocID) AS TotalDocuments --SUM(d.DocID) adds the actual ID numbers together. If you have two documents with IDs 10 and 11, your result will be 21.
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.LawyerID, l.Name;

--answer:
SELECT l.LawyerID, l.Name, COUNT(d.DocID) AS DocCount --COUNT(d.DocID) counts how many records exist. For IDs 10 and 11, the result will be 2.
FROM Lawyers l 
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID 
LEFT JOIN Documents d ON m.MatterID = d.MatterID 
GROUP BY l.LawyerID, l.Name;

--Your Query (JOIN): This only shows lawyers who have at least one matter and one document. If a lawyer is new and has no cases yet,
-- they are completely deleted from your results.
-- The Previous Solution (LEFT JOIN): This keeps every lawyer in the list. If they have no documents, they stay in the list with a count of 0.
-- Interview Tip: In a reporting scenario, managers usually want to see everyone (including those with 0), so LEFT JOIN is safer.


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

--Q4: Filtered Aggregates (Where vs. Having)
-- 4. List Matters that have a total file size greater than 10,000 KB, but ignore any individual documents that are smaller 
-- than 100 KB in that calculation.

SELECT m.Title, SUM(CASE WHEN d.FileSizeKB >= 10000 THEN d.FileSizeKB END) AS TotalFileSize
FROM Matters m 
JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY m.Title
HAVING SUM(d.FileSizeKB) > 10000;