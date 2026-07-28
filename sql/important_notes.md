in a join question how do i differentiate between using an inner join or left join?

I know when they say the right columns data can be empty/null or they refer to it


1. The "Every/All" Rule (LEFT JOIN)
If the question asks for "all" of one category, regardless of whether they have a relationship, use a LEFT JOIN.
Keywords: "All lawyers, even those without cases," "Every department, including empty ones," "List all users and their orders (if any)."
Logic: You are protecting the data in the "Left" table from being filtered out.
2. The "Only/Matching" Rule (INNER JOIN)
If the question asks for a list where both sides must exist, use an INNER JOIN.
Keywords: "List matters and their lawyers," "Show me documents assigned to a matter," "Who is the lawyer for Case X?"
Logic: If a Matter doesn't have a Lawyer, it shouldn't show up at all.
3. The "Missing/Not In" Rule (LEFT JOIN + NULL)
If the question asks you to find things that are missing or unassigned, it is always a LEFT JOIN.
Keywords: "Which lawyers have no matters?", "Find matters with zero documents."
Logic: You join them, then look for the "empty" spots where the match failed.
4. The "Integrity" check
If you know for a fact (via the schema) that a column is NOT NULL (mandatory), an INNER JOIN and LEFT JOIN will actually return the exact same result.
Example: If every Matter must have a LeadLawyerID, then Matters JOIN Lawyers is the same as Matters LEFT JOIN Lawyers.

a LEFT JOIN is more "expensive" for the database than an INNER JOIN, and that you only use it when strictly necessary.



Tip:
If they ask you to filter based on an aggregate (Sum, Count, Avg), always reach for HAVING. If they ask you to filter based on a specific row property, use WHERE.



The Bottom Line:
Mastering Inner and Left Joins is 80% of the battle. The other 20%—which is what the interview focuses on—is knowing when to use which one and how to keep the query fast as the database grows to millions of records.




If an interviewer asks you to filter based on a number of occurrences (like "more than 1", "at least 5"), your brain should immediately think: "Group By + Having Count."


tips to avoid duplicates:

- GROUP BY the column id
- select DISTINCT





Rule of Thumb: Use SUM for values (money, weight, sizes) and COUNT for tracking "how many" items there are.

Using COUNT(Quantity) instead of SUM(Quantity) is one of the most common mistakes candidates make when transitioning from Junior to Intermediate roles.
Here is why that mistake changes the logic of your code, and how to keep it straight in your notes:The Difference in Output

Let's look at a single order that contains two items:
Item 1: 2 laptops
Item 2: 5 mice

Function:            How the Database Thinks:                                                   Your Result: 
COUNT(Quantity)    "How many rows or entries are there in this group?"                         2 (Because there are two separate rows of items)
SUM(Quantity)      "What is the math total of the numbers in this column?"               7 (\(2 + 5\) total items shipped)


If the question asks for a specific metric threshold (e.g., HAVING SUM(x) > 500 or HAVING AVG(y) < 50), use an INNER JOIN to optimize performance.



Use DISTINCT when you are jumping "up" a relationship to find unique parents (e.g., "Find unique CustomerNames from the Orders table").
Do NOT use DISTINCT when you are listing items from a base inventory table (like Products), because each individual item should be allowed to show up on its own row.
refer to practice2.sql file answer 6

You have to use GROUP BY when you use an aggregate function like COUNT(), SUM(), or AVG() alongside a regular column (like m.Title), SQL needs to know how to group the individual rows.




In SQL, the physical order of your JOIN statements dictates what table aliases are available. You can only use a table's alias if that table has already been introduced in the query above or on that exact line.

Broken Chain: Orders \(\rightarrow \) OrderItems \(\rightarrow \) [Tries to use Products column] \(\rightarrow \) Categories \(\rightarrow \) Products
Correct Chain: Orders \(\rightarrow \) OrderItems \(\rightarrow \) Products \(\rightarrow \) Categories

-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

--4. List the Names of Customers who have ordered products from the 'Electronics' category.

SELECT DISTINCT o.CustomerName
FROM Orders o 
JOIN OrderItems oi ON o.OrderID = oi.OrderID
JOIN Categories c ON c.CategoryID = p.CategoryID
JOIN Products p ON c.CategoryID = p.CategoryID
WHERE c.CategoryName = 'Electronics';

SELECT DISTINCT o.CustomerName
FROM Orders o
JOIN OrderItems oi ON o.OrderID = oi.OrderID
JOIN Products p ON oi.ProductID = p.ProductID
JOIN Categories c ON c.CategoryID = p.CategoryID
WHERE c.CategoryName = 'Electronics';



https://www.youtube.com/watch?v=rIcB4zMYMas
Window functions:

The function does the math or counting, while the OVER() clause defines who is included in that calculation.

The OVER clause defines the 'window' (or subset) of data the function calculates across, allowing you to run aggregate math without collapsing your individual rows.

SELECT Gender, Name, Total

    //Function                            //Window
    //1..2..3 in popularity column rows   // how you wanna view your data when applying your function
    ROW_NUMBER()                          OVER(ORDER BY Total DESC)                                       AS Popularity

FROM baby_names



//split the gender column into boy and girl group vertically in the gender column - use PARTITION BY

SELECT Gender, Name, Total

    //Function                            //Window
    //1..2..3 in popularity column rows   // how you wanna view your data when applying your function
    ROW_NUMBER()                          OVER(PARTITION BY Gender ORDER BY Total DESC)                                       AS Popularity
    
FROM baby_names




-- Schema Details:
-- Lawyers (LawyerID, Name, Department)
-- Matters (MatterID, Title, LeadLawyerID)
-- Documents (DocID, MatterID, FileSizeKB)

----5. List all Lawyers and any Matters they lead with 'Litigation' in the title. Lawyers with no 'Litigation' matters must still 
-- appear in the list.

-------WRONG----------
SELECT l.Name, m.Title
FROM Lawyers l 
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID
WHERE m.Title = 'Litigation'

SELECT l.Name, m.Title
FROM Lawyers l
LEFT JOIN Matters m ON l.LawyerID = m.LeadLawyerID 
     **AND m.Title LIKE '%Litigation%';**

--Filter in ON: Filters the "right" table before the join. Keeps all rows from the "left" table.
--Filter in WHERE: Filters the entire result after the join. Can accidentally delete "left" table rows.

**putting '' around a string value is used to match an exact text string, 
Operator used: Equals sign (=)
Performance: Ultra-fast, especially if the column is indexed.

**putting percentage signs (%) inside quotation marks is a wildcard tool used for partial text matching (searching).
Operator used: You must use the LIKE operator. If you use = with percentage signs (e.g., WHERE Title = '%Hardware%'), SQL will literally search for text containing actual percent signs.
Performance: Slower. It forces SQL Server to perform a full-table scan (reading every single row) because it cannot use standard index sorting trees effectively.
**find out how to optimize/make it SARGable a wildcard partial text matching query**


-- Categories (CategoryID, CategoryName)
-- Products (ProductID, ProductName, CategoryID, Price)
-- Orders (OrderID, OrderDate, CustomerName)
-- OrderItems (ItemID, OrderID, ProductID, Quantity)

--5. Find Orders where the total quantity of items is greater than 10

SELECT oi.OrderID, COUNT(oi.Quantity) AS QuantityCount
FROM OrderItems oi
GROUP BY oi.OrderID
HAVING COUNT(oi.Quantity) > 10;
-- didnt have to join here despite answer query having a join. Because OrderItems table contains OrderID and Quantity
-- If an interviewer gives you a question like Q5 and you catch this shortcut, you immediately stand out as an Intermediate Engineer 
-- because you are actively thinking about Performance and Efficiency.
-- Cuts database work in half. It reads from a single table, bypassing the expensive join overhead entirely.



-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--1. Find the FullName and SubmissionDate of all candidates who submitted their details in January 2026.

SELECT FullName, SubmissionDate
FROM Candidates
WHERE SubmissionDate >= '2026-01-01'
    AND SubmissionDate < '2026-02-01';

Using an asymmetrical range (>= '2026-01-01' AND < '2026-02-01') instead of a LIKE operator or pulling the month out via a function (like MONTH()) is the exact engineering best practice LexisNexis looks for. It keeps the query sargable, meaning the database engine can fully utilize an index on the SubmissionDate column.



-- Schema Details:
-- Clients (ClientID, CompanyName, Industry)
-- Candidates (CandidateID, ClientID, FullName, SubmissionDate)
-- Verifications (CheckID, CandidateID, CheckType, CostZAR, Status)
-- VerificationLogs (LogID, CheckID, ActionTaken, LogTimestamp)

--6. Return the CandidateID and FullName of any candidate who has at least one check with a 'Pending' status, but completely exclude candidates who have any checks 
-- with a 'Flagged' status.

--my try:
-- SELECT c.CandidateID, c.FullName
-- FROM Candidates c 
-- JOIN Verifications v ON c.CandidateID = v.CandidateID
-- GROUP BY c.CandidateID, c.FullName
-- HAVING NOT v.Status;
-- WHERE v.Status = 'Pending' AND v.Status != 'Flagged';

--shortest solution i found
SELECT DISTINCT 
    c.CandidateID, 
    c.FullName
FROM Candidates c
JOIN Verifications p ON c.CandidateID = p.CandidateID AND p.Status = 'Pending' -- filters candidate pool to only include individuals who have at least one 'Pending' check
LEFT JOIN Verifications f ON c.CandidateID = f.CandidateID AND f.Status = 'Flagged' --find only the 'Flagged' checks for those exact same candidates.
WHERE f.CandidateID IS NULL; --drop anyone who has a flagged status