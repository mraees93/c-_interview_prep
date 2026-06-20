SELECT Status, COUNT(LoanID), AVG(Amount)
FROM Loans
WHERE Amount > 1000
GROUP BY Status;
--What is the operational purpose of the GROUP BY Status clause in this specific database query?
--answer:
--It consolidates rows that share identical Status values into single summary rows so the count and 
--average can be calculated per status type.


--An engineer wants to find all loan statuses where the average loan amount is greater than R5,000. They write the following query:
SELECT Status, AVG(Amount)
FROM Loans
WHERE AVG(Amount) > 5000
GROUP BY Status;
--What happens when this SQL query is executed against the database server?
--answer:
--It throws an execution error because aggregate functions like AVG() cannot be used inside a standard WHERE clause; 
--they require a HAVING clause.
SELECT Status, AVG(Amount)
FROM Loans
GROUP BY Status
HAVING AVG(Amount) > 5000;



