-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

--1. List all Categories and the number of Products in each. Include categories with zero products.

SELECT c.CategoryName, COUNT(p.ProductID) AS ProductsCount
FROM Categories c
LEFT JOIN Products p ON c.CategoryID = p.CategoryID
GROUP BY c.CategoryID, c.CategoryName;

--2. Find Products that have never been ordered.

SELECT p.ProductName 
FROM Products p
LEFT JOIN OrderItems oi ON p.ProductID = oi.ProductID
WHERE oi.ItemID IS NULL;

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

SELECT DISTINCT l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
WHERE d.DocID IS NULL;

SELECT DISTINCT l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
LEFT JOIN Documents d ON m.MatterID = d.MatterID
WHERE d.DocID IS NULL;