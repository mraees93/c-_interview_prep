Any derived class should work in place of a parent class without any changes.
And without affecting the correctness of the program.

-Liskov's principle applies to interfaces exactly the same way it applies to classes. An interface acts as the "parent contract," and the class that implements it acts as the "child.

- This principle ensures that inheritance hierarchies are well-designed and that 
subclasses adhere to the contracts defined by their superclasses.


What are the common 'code smells' that suggest an LSP violation?

Using is or as keywords to check the specific type of a subclass before calling a method (e.g., if (bird is Penguin)).
Empty or "No-Op" method overrides.
Methods that return "Magic Values" to indicate they can't perform an action (e.g., returning -1 for a method that usually returns a positive ID).

pro tip: Explain that these checks make code brittle. If you add a 10th subclass, you’d have to find every if/else check in the system to update it.


**The golden architectural rule behind fixing LSP violations:**

If you find yourself writing if (obj is SubclassA) in your main logic, you are likely violating LSP. The calling code should be "blind" to which specific subclass it is using. 
When a child class cannot fulfill every behavioral expectation (preconditions, postconditions, and side effects) of its parent, you do not force it to fit with if checks, early returns, or NotSupportedException. Instead, you extract a separate parent abstraction (a new interface or base class) tailored to that child's true behavior so that every child is a 100% honest substitute for its parent.


- Violations of the LSP can lead to unexpected behavior or errors when substituting objects, 
making code harder to reason about and maintain.


**check prac_19.cs**
there can be a fix of multiple parent/child group substitutes; if the child isnt a perfect substitute then create a new parent to force the child to be a perfect substitute.




**check practice_5.cs**
In C#, when you find yourself passing multiple parameters to represent a single real-world entity (like passing three objects to represent one person checking in), it is almost always a sign that your base class or interface abstractions are split incorrectly.

Can a client class use the parent contract without knowing which specific child it is holding? If the answer is yes, you have achieved a perfect LSP design.

Mental Checklist:
Parent: Only holds behaviors that are 100% universal across all variations.
Child: Only inherits what it can genuinely execute without throwing a NotSupportedException.
Side-Interfaces: House the extra "premium" or optional features that not every child needs.

Why LSP is the Hardest to Spot:
The Compiler Won't Help You: The compiler only cares if your syntax matches. It doesn't know that a ceiling fan shouldn't have a volume slider or that a fixed deposit account shouldn't allow standard deposits.
Inheritance is a Trap: We are taught that inheritance means an "is a" relationship (e.g., a Square is a Rectangle, a Fan is an Electronic Device). LSP teaches us that inheritance must be based on behavioral compatibility, not just vocabulary.

• The Diagnostic Question: "If I swap Child B into any method expecting Parent A, will it secretly skip work, mutate unexpected state, or throw an exception?"
• The Architectural Fix: If yes, split the hierarchy. Create Parent A (for the standard behavior) and Parent B (for the specialized behavior) so every child is a true, predictable substitute for its parent.