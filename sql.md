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




Schema Details:
Lawyers (LawyerID, Name, Department)
Matters (MatterID, Title, LeadLawyerID)
Documents (DocID, MatterID, FileSizeKB)

1. Show every Matter title and the name of the Lawyer leading it.

SELECT m.Title, l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LawyerID

SELECT m.Title, l.Name 
FROM Matters m 
JOIN Lawyers l ON m.LeadLawyerID = l.LawyerID;

*tip:
In a database, you make Matters.LeadLawyerID point to Lawyers.LawyerID using a Foreign Key constraint. This tells the database that every ID entered in the Matters table must already exist in the Lawyers table.


Schema Details:
Lawyers (LawyerID, Name, Department)
Matters (MatterID, Title, LeadLawyerID)
Documents (DocID, MatterID, FileSizeKB)

2. Find all Lawyers who do not currently have any Matters assigned to them.

Mine:
SELECT l.Name, m.Title
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
WHERE m.Matters IS NULL; *** i was referring to a table alias instead of a column

answer:
SELECT l.Name
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
WHERE m.MatterID IS NULL;

*tip:
To find records that have no match in a LEFT JOIN, always pick the Primary Key of the right-hand table

3. List every Document ID alongside its Matter title and the Lead Lawyer's name.

SELECT d.DocID, m.Title, l.Name
FROM Lawyers l
JOIN Matters m ON l.LawyerID = m.LeadLawyerID
JOIN Documents d ON d.MatterID = m.MatterID;


Schema Details:
Lawyers (LawyerID, Name, Department)
Matters (MatterID, Title, LeadLawyerID)
Documents (DocID, MatterID, FileSizeKB)

4. Calculate the total sum of FileSizeKB for each Lawyer based on the matters they lead.

join the 3 tables based on relationships
use group by based on matters group
calculate sum by group