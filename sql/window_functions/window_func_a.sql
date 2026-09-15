-- 1. Select the TransactionID, EmployeeID, SalePrice, and TransactionDate, alongside a column that assigns a sequential row number to every transaction across the 
-- entire database, ordered from the newest transaction date to the oldest.

SELECT TransactionID, EmployeeID, SalePrice, TransactionDate,
        ROW_NUMBER() OVER(ORDER BY TransactionDate DESC) AS TransactionSequence
FROM Transactions;

-- 2. Display the EmployeeID, FullName, Role, and HireDate, along with a column that assigns a rank to employees within each specific Role based on their 
-- seniority (earliest HireDate gets rank 1). If there is a tie, they must share the same rank number, and the next rank number in the sequence must skip ahead.

SELECT EmployeeID, FullName, Role, HireDate,
      RANK() OVER(PARTITION BY Role ORDER BY HireDate ASC) AS HireDateSequence
FROM Employees;

-- 3. Find the TransactionID, StoreID, SalePrice, and TransactionDate, along with a column showing the highest individual SalePrice recorded within that 
-- specific transaction's StoreID up until that point in time (ordered chronologically by TransactionDate).

SELECT TransactionID, StoreID, SalePrice, TransactionDate,
      MAX(SalePrice) OVER(PARTITION BY StoreID ORDER BY TransactionDate ASC) AS HistoricalMaxSalePrice
FROM Transactions;

-- Schema Details:
-- Stores (StoreID, StoreName, City)
-- Employees (EmployeeID, StoreID, FullName, Role, HireDate)
-- SalesBooks (BookID, Title, Category, BasePrice)
-- Transactions (TransactionID, StoreID, EmployeeID, BookID, SalePrice, TransactionDate)

-- 4. Select the EmployeeID, StoreID, Role, and HireDate, along with a dense ranking that orders employees inside each StoreID by their length of 
-- service (longest-serving employee gets rank 1), regardless of their Role.

SELECT EmployeeID, StoreID, Role, HireDate,
       DENSE_RANK() OVER(PARTITION BY StoreID ORDER BY HireDate ASC) AS EmployeesServiceRank
FROM Employees;

-- 5. Display the BookID, Title, Category, and BasePrice, alongside a column showing the average BasePrice of all books belonging to that specific book's Category. 
-- Do not use a GROUP BY clause.

SELECT BookID, Title, Category, BasePrice,
       AVG(BasePrice) OVER(PARTITION BY Category) AS AvgBasePrice
FROM SalesBooks;

-- 6. Write a query to show every TransactionID, StoreID, BookID, SalePrice, and a column calculating the exact difference between the 
-- transaction's individual SalePrice and the overall average SalePrice of that specific StoreID.

-- inline approach
SELECT TransactionID, StoreID, BookID, SalePrice,
       (SalePrice - AVG(SalePrice) OVER(PARTITION BY StoreID)) AS PriceDifference
FROM Transactions;

-- CTE approach:
WITH AvgSalePrices AS (
    SELECT TransactionID, StoreID, BookID, SalePrice,
           AVG(SalePrice) OVER(PARTITION BY StoreID) AS AvgSalePrice
    FROM Transactions
)
SELECT TransactionID, StoreID, BookID, SalePrice,
       (SalePrice - AvgSalePrice) AS PriceDifference
FROM AvgSalePrices;