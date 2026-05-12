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





------------------------------------------------------ 7 double check ------------------------------------------------------------------------
-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 4. List Matters that have a total file size greater than 10,000 KB, 
--but ignore any individual documents that are smaller than 100 KB in that calculation.

--try:
SELECT m.Title, SUM(CASE WHEN d.FileSizeKB >= 10000 THEN d.FileSizeKB END) AS TotalFileSize
FROM Matters m 
JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY m.Title
HAVING SUM(d.FileSizeKB) > 10000;

--answer:
SELECT m.Title, SUM(d.FileSizeKB) AS TotalLargeFiles
FROM Matters m
JOIN Documents d ON m.MatterID = d.MatterID
WHERE d.FileSizeKB >= 100        -- Step 1: Ignore files < 100KB immediately
GROUP BY m.Title                 -- Step 2: Group the remaining "large" files
HAVING SUM(d.FileSizeKB) > 10000; -- Step 3: Check if their sum is > 10,000

--Use WHERE when you want to exclude specific items from a calculation (like "ignore small files").
--Use HAVING when you want to exclude entire groups based on the final result (like "ignore cases that are small overall").


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

--5. List all Lawyers and any Matters they lead with 'Litigation' in the title. Lawyers with no 'Litigation' matters must still 
-- appear in the list.

--mine:
SELECT l.Name, m.Title
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
WHERE m.Title LIKE '%Litigation%';

--answer:
SELECT l.Name, m.Title
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID 
     AND m.Title LIKE '%Litigation%';

--Filter in ON: Filters the "right" table before the join. Keeps all rows from the "left" table.
--Filter in WHERE: Filters the entire result after the join. Can accidentally delete "left" table rows.


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

--6. Find the average document size (FileSizeKB) for each Department.

SELECT l.Department, AVG(d.FileSizeKB) AS AvgDocSize
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.Department;


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 7. Find the single largest document (highest FileSizeKB) for each Department. Show the Department name, the Document ID, and the size.
--my try:
SELECT l.Department, d.DocID, MAX(d.FileSizeKB) AS MaxFileSize
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.Department, d.DocID;

--answer:
SELECT Department, DocID, FileSizeKB
FROM (
    SELECT 
        l.Department, 
        d.DocID, 
        d.FileSizeKB,
        ROW_NUMBER() OVER(
            PARTITION BY l.Department  -- Splits data into "piles" by Department
            ORDER BY d.FileSizeKB DESC -- Sorts each "pile" largest to smallest
        ) as rnk                       -- Assigns a "ticket number" (#1 is largest)
    FROM Lawyers l
    JOIN Matters m ON l.LawyerID = m.LeadLawyerID
    JOIN Documents d ON m.MatterID = d.MatterID
) t
WHERE rnk = 1;                         -- Keeps only the #1 (largest) from each pile

-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

--8. List all Lawyers who lead more than one Matter in the 'Litigation' department.
--try:
SELECT l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
WHERE m.Title LIKE '%Litigation%'
AND m.Title > 1;

--answer:
SELECT l.Name, COUNT(m.MatterID) AS LitigationMatters
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
WHERE l.Department = 'Litigation'   -- Step 1: Filter the rows first
GROUP BY l.LawyerID, l.Name         -- Step 2: Group them by lawyer
HAVING COUNT(m.MatterID) > 1;       -- Step 3: Filter the groups after counting

--you can absolutely use l.Department in the WHERE clause and l.LawyerID in the GROUP BY even if they 
--aren't "part of the join" (meaning they aren't the columns used to link the tables).

--The WHERE clause can use any column from any table listed in your FROM or JOIN statements.
--Because you have Lawyers l in your query, you have full access to all its columns (Name, Department, LawyerID) to filter the data.

--The GROUP BY Clause You can group by any column from the joined tables. 
--In fact, grouping by l.LawyerID is highly recommended for an intermediate role:The "Duplicate Name" Problem: 
--If you only grouped by l.Name and you had two different lawyers named "John Smith," the database would merge their matters together.
--The Safety Net: By grouping by the Primary Key (LawyerID), you ensure each lawyer's count is kept separate, even if they share a name.


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

--9. Calculate the percentage of the total system storage (FileSizeKB) that each Department is responsible for.

SELECT 
     l.Department, 
     SUM(d.FileSizeKB) AS TotalSystemStorage,
     (SUM(d.FileSizeKB) * 100.0 / SUM(SUM(d.FileSizeKB)) OVER()) AS Percentage
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
JOIN Documents d ON m.MatterID = d.MatterID
GROUP BY l.Department


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 10. Identify Matters where the Lead Lawyer belongs to a different Department than the Matter's title would suggest. 
-- (Specifically: Find Matters with 'Corporate' in the title, but led by a Lawyer in the 'Litigation' department).

SELECT m.Title, l.Name, l.Department
FROM Matters m
JOIN Lawyers l ON m.LeadLawyerID = l.LawyerID -- Links the matter to its lead lawyer
WHERE m.Title LIKE '%Corporate%'               -- Filters for specific matter keyword
  AND l.Department = 'Litigation';             -- Filters for the "mismatched" department
