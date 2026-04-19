Practice Scenario: Sales Management System

Schema Details:
Employees (ID, Name, DeptID, Salary)
Departments (DeptID, DeptName) //child of Employees

Sales (SaleID, EmployeeID, ProductID, Amount, SaleDate)
Products (ProductID, ProductName) // child of Sales

1. Intermediate Joins & Unmatched Records
Question: Write an SQL query to retrieve all employee names and their department names. Ensure you include employees who have not been assigned to a department yet.

Mine:

SELECT Employees.Name, Departments.DeptName
FROM Employees
LEFT JOIN Departments ON Employees.DeptID = Departments.DeptID

Industry standard (It keeps the query concise, especially when you have long table names or are joining 4+ tables. It also saves you a lot of typing!):

SELECT e.Name, d.DeptName
FROM Employees e
LEFT JOIN Departments d ON e.DeptID = d.DeptID;


2. Aggregation with Filtering (HAVING clause)
Question: Find the names of departments that have an average salary greater than $50,000.

SELECT d.DeptName FROM Employees e
JOIN Departments d ON d.DeptID = e.DeptID
GROUP BY d.DeptName
HAVING AVG(e.Salary) > 50000;
