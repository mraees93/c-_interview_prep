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
     AND m.Title LIKE '%Litigation%';

--Filter in ON: Filters the "right" table before the join. Keeps all rows from the "left" table.
--Filter in WHERE: Filters the entire result after the join. Can accidentally delete "left" table rows.