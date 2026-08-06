-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

----5. List all Lawyers and any Matters they lead with 'Litigation' in the title. Lawyers with no 'Litigation' matters must still 
-- appear in the list.

SELECT l.Name, m.Title
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
    AND m.Title LIKE '%Litigation%';

SELECT l.Name, m.Title
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID 
     AND CONTAINS(m.Title, '%Litigation%');



-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

--5. Find Orders where the total quantity of items is greater than 10

SELECT OrderID, COUNT(Quantity) AS TotalQuantity
FROM OrderItems
GROUP BY OrderID
HAVING COUNT(Quantity) > 10;

-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--6. Return the CandidateID and FullName of any candidate who has at least one check with a 'Pending' status, but completely exclude candidates who have any checks 
-- with a 'Flagged' status.

SELECT c.CandidateID, c.FullName
FROM Candidates c
JOIN Verifications p ON c.CandidateID = p.CandidateID AND p.Status = 'Pending'
LEFT JOIN Verifications f ON c.CandidateID = f.CandidateID AND f.Status = 'Flagged'
WHERE f.CandidateID IS NULL;

SELECT DISTINCT 
    c.CandidateID, 
    c.FullName
FROM Candidates c
JOIN Verifications p ON c.CandidateID = p.CandidateID AND p.Status = 'Pending' -- filters candidate pool to only include individuals who have at least one 'Pending' check
LEFT JOIN Verifications f ON c.CandidateID = f.CandidateID AND f.Status = 'Flagged' --find only the 'Flagged' checks for those exact same candidates.
WHERE f.CandidateID IS NULL; --drop anyone who has a flagged status

--7. Find the most expensive Product in each Category (using a Window Function).

-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

WITH SortedProducts AS (
    SELECT c.CategoryName, p.ProductName, p.Price,
        ROW_NUMBER() OVER(PARTITION BY p.CategoryID ORDER BY p.Price DESC) AS rank
    FROM Categories c
    JOIN Products p ON c.CategoryID = p.CategoryID
)
SELECT CategoryName, ProductName, Price
FROM SortedProducts
WHERE rank = 1;