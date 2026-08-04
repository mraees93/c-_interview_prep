-- 1. List all Lawyers and their total file storage. If a lawyer has no files, show 0 instead of NULL

-- 2. Find the Names of Lawyers who have at least one Matter, but that Matter has zero documents.


-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

-- 3. Calculate total documents per Lawyer, ensuring that if two lawyers have the same name, they are not combined.


-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

--5. Find Orders where the total quantity of items is greater than 10


--9. Write a query to show the CandidateID, CheckType, and CostZAR, alongside a new column displaying the average cost of that specific CheckType across the entire 
-- database.


-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--10. Select the CandidateID, FullName, and SubmissionDate, along with a column that assigns a sequential row number to candidates for each unique client, 
-- ordered by their SubmissionDate from earliest to latest.
