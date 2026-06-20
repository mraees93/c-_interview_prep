Consider two database tables inside Wonga's lending system:
Customers (contains 100 unique customer rows)
Loans (contains 80 rows tracking loans linked to those customers)

If you execute an INNER JOIN query to connect these tables on the matching customer ID column, what is the maximum number of rows that the query can return?
B. Exactly 80 rows.



An engineer runs a LEFT JOIN query starting from the Customers table and linking over to the Loans table. If a specific customer in the database has never taken out a loan, what values will appear in the columns belonging to the Loans table for that customer's output row?

The columns will contain NULL values.       



What is the core structural difference between a Primary Key and a Unique Constraint inside a relational SQL database table?

A table can have multiple Unique Constraints, but it can only ever have one Primary Key. Additionally, a Primary Key completely forbids NULL values.



Why do full-stack developers apply a Non-Clustered Index to a specific column (like an identity number or cell phone number field) inside a high-volume database table?

To speed up data retrieval searches (SELECT queries) matching on that column, at the expense of slightly slowing down data updates and inserts (INSERT/UPDATE operations).



Consider a legacy .NET 4.7 data access method that connects to an enterprise database server:
public void FetchSystemLogs()
{
    SqlConnection conn = new SqlConnection("Server=WongaDB;Database=Logs;Integrated Security=True;");
    conn.Open();
    SqlCommand cmd = new SqlCommand("SELECT * FROM AuditLogs", conn);
    SqlDataReader reader = cmd.ExecuteReader();
    // Process records here...
}
What is the primary architectural defect found inside this backend code snippet?

The database connection (conn) and reader objects are never explicitly closed or wrapped inside a using block, causing connection leaks in the application's Connection Pool.

Wrapping unmanaged connections inside a using statement guarantees that the connection is safely closed, even if an unexpected error occurs during data processing:
using (SqlConnection conn = new SqlConnection(connectionString))
{
    conn.Open();
    // Your execution logic here...
} // conn.Dispose() is automatically fired here!



Why must full-stack engineers utilize Parameterized Queries (like cmd.Parameters.AddWithValue()) rather than concatenating user input strings directly into raw SQL text statements?
To completely prevent SQL Injection security vulnerabilities by ensuring the database engine treats input values strictly as data constants, rather than executable code instructions.

The Security Threat: If you build a query using basic string concatenation (like "SELECT * FROM Users WHERE Name = '" + userInput + "'"), a malicious user could type a piece of database command text into your application's input box—for example: ' OR 1=1; DROP TABLE Customers; --

The Resolution: Parameterization separates the code layout from the data value. When you use parameters, the database compiler processes the SQL structure first, and then slots your input safely into place as a plain string constant. Even if a user inputs database command keywords, the engine treats it strictly as a literal text string value, completely preventing SQL Injection.



What is the primary operational benefit of wrapping multiple database data modifications inside a formal SQL Transaction (BEGIN TRANSACTION ... COMMIT ... ROLLBACK) block?

 It guarantees Atomicity (All-or-Nothing execution). If three database tables need to be updated, and the third update fails, the transaction rolls back the first two updates automatically, preventing broken data states

The Problem: Imagine a user makes a payment. Your system needs to do two things: deduct R500 from the UserAccount table AND add a new row to the TransactionHistory table. If the computer crashes right after updating the account table but before writing the history log, your data becomes completely corrupt—money has vanished without a trace.
The Solution: Wrapping these statements in a transaction forces them to act as a single, atomic unit. If any part of the process hits an error, the database fires a ROLLBACK, undoing every single change made since the transaction started. Your database returns to its clean, original state, ensuring total data integrity.



What is the behavioral result of executing the lone throw; keyword inside this catch block when an external network service failure occurs?
try
{
    // External third-party payment vendor API call occurs here
    _paymentService.ProcessTransaction();
}
catch (WebException ex)
{
    // Log exception metrics here
    throw;
}

It re-throws the original exception up the call stack while fully preserving the original stack trace data, allowing developers to track exactly where the root failure occurred.

The Difference (The Secret Trap): In C#, there is a massive operational difference between writing throw; and writing throw ex;.

Writing throw ex;: This is a common developer mistake. Doing this resets the stack trace. The application pretends that the error originated right there inside your catch block, completely wiping out the history of what went wrong deep inside the external service method.

Writing throw;: By using the standalone keyword exactly as you selected, you preserve the original Stack Trace. The exception bubbles up to parent error handlers keeping all original file names and line numbers intact, allowing developers to trace the bug right back to its original line inside the payment service logic.