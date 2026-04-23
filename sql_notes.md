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
