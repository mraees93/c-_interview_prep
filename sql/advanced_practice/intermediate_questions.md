-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--1. Find the FullName and SubmissionDate of all candidates who submitted their details in January 2026.

--6. Return the CandidateID and FullName of any candidate who has at least one check with a 'Pending' status, but completely exclude candidates who have any checks 
-- with a 'Flagged' status.

--9. Write a query to show the CandidateID, CheckType, and CostZAR, alongside a new column displaying the average cost of that specific CheckType across the entire 
-- database.

--10. Select the CandidateID, FullName, and SubmissionDate, along with a column that assigns a sequential row number to candidates for each unique client, 
-- ordered by their SubmissionDate from earliest to latest.

-- 12. Show the CheckID, CandidateID, CostZAR, and a dense ranking of the check costs within each specific CheckType, where the most expensive check gets a rank of 1.

--7. Find the most expensive Product in each Category (using a Window Function).

-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)