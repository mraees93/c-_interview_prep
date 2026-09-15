-- Schema Details:
-- Stores (StoreID, StoreName, City)
-- Employees (EmployeeID, StoreID, FullName, Role, HireDate)
-- SalesBooks (BookID, Title, Category, BasePrice)
-- Transactions (TransactionID, StoreID, EmployeeID, BookID, SalePrice, TransactionDate)

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