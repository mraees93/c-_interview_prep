Practice Scenario: Sales Management System

Schema Details:
Departments (DeptID, DeptName) 
Employees (ID, Name, DeptID, Salary) //child of Departments coz DeptID (foreign key) refers to Departments DeptID primary key

Products (ProductID, ProductName) 
Sales (SaleID, EmployeeID, ProductID, Amount, SaleDate) //child of Products coz ProductID (foreign key) refers to Products ProductID primary key

1. Intermediate Joins & Unmatched Records
Question: Write a SQL query to retrieve all employee names and their department names. Ensure you include employees who have not been assigned to a department yet.

// for left joins Employees and Departments cant be swapped (in the FROM and LEFT JOIN lines) but it can be swapped in the ON line
SELECT Employees.Name, Departments.DeptName
FROM Employees
LEFT JOIN Departments ON Departments.DeptID = Employees.DeptID

SELECT e.Name, d.DeptName
FROM Employees e 
LEFT JOIN Departments d ON d.DeptID = e.DeptID

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


Schema Details:
Departments (DeptID, DeptName) 
Employees (ID, Name, DeptID, Salary) //child of Departments coz DeptID (foreign key) refers to Departments DeptID primary key

Products (ProductID, ProductName) 
Sales (SaleID, EmployeeID, ProductID, Amount, SaleDate) //child of Products coz ProductID (foreign key) refers to Products ProductID primary key

answer:
SELECT d.DeptName FROM Employees e
JOIN Departments d ON d.DeptID = e.DeptID
GROUP BY d.DeptName
HAVING AVG(e.Salary) > 50000;

my try:
SELECT d.DeptName, e.Salary
FROM Departments d
JOIN Employees e ON d.DeptID = e.DeptID
WHERE e.Salary > 50 000
/*
missed the mark because you filtered individuals instead of measuring the group.
Here is the breakdown of where it went wrong:
Wrong Level of Data: Your WHERE clause looks at individual salaries. The question asks for a department average.
No Aggregation: To find an "average," you must use the AVG() function and GROUP BY. Your query treats every row as a separate entity.
WHERE vs. HAVING: You used WHERE, which filters rows before any math happens. To filter based on the result of an average, you must use HAVING after the math is done.
Syntax Error: You wrote 50 000 with a space. SQL requires numbers to be continuous, like 50000.
In short: Your query answers "Who earns a lot?" while the goal was to answer "Which departments are expensive on average?"
*/




3. Return a list of all customers (Names and Emails) who have spent more than R5,000 in total across all their orders, but only include orders that are currently marked as 'Shipped'.