If you see these in code, the "substitution" is broken:
1. The "Refusal of Bequest" (Throwing Exceptions)
This is the most common violation. It happens when a child class inherits from a parent but "refuses" to implement a method by throwing a NotImplementedException or InvalidOperationException.
The Cause: Forcing a child into a hierarchy where it doesn't belong.
The Result: The calling code expects a certain behavior (based on the parent's contract) but gets a crash instead.
2. The "Type Checking" Smell (Violating Polymorphism)
If you find yourself using is, as, or .GetType() to check which specific child class you are holding, you have violated LSP.
The Cause: The parent class is not generic enough to represent all its children, so the developer has to write "special case" logic for specific subclasses.
The Result: The code becomes rigid. Every time you add a new subclass, you have to go back and update all your if/else or switch statements.
3. Strengthening Pre-conditions
This is the "academic" side of LSP. It happens when a child class adds stricter rules to its inputs than the parent class had.
The Cause: A parent method accepts any int, but the child overrides it and throws an error if the int is negative.
The Result: Code that worked perfectly with the parent suddenly fails when the child is substituted in, because the child is "pickier" than its parent.

One-Sentence Summary for the Interviewer:
"LSP is violated whenever a subclass changes the expected behavior of the parent, forcing the calling code to know exactly which concrete implementation it is dealing with."