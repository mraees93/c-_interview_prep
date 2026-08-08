Any derived class should work in place of a parent class without any changes.
And without affecting the correctness of the program.

-Liskov's principle applies to interfaces exactly the same way it applies to classes. An interface acts as the "parent contract," and the class that implements it acts as the "child.

- This principle ensures that inheritance hierarchies are well-designed and that 
subclasses adhere to the contracts defined by their superclasses.

- Violations of the LSP can lead to unexpected behavior or errors when substituting objects, 
making code harder to reason about and maintain.


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

