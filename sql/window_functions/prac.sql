-- Schema Details:
-- Stores (StoreID, StoreName, City)
-- Employees (EmployeeID, StoreID, FullName, Role, HireDate)
-- SalesBooks (BookID, Title, Category, BasePrice)
-- Transactions (TransactionID, StoreID, EmployeeID, BookID, SalePrice, TransactionDate)

-- 3. Find the TransactionID, StoreID, SalePrice, and TransactionDate, along with a column showing the highest individual SalePrice recorded within that specific 
-- transaction's StoreID up until that point in time (ordered chronologically by TransactionDate).