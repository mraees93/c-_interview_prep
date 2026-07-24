Any derived class should work in place of a parent class without any changes.
And without affecting the correctness of the program.

-Liskov's principle applies to interfaces exactly the same way it applies to classes. An interface acts as the "parent contract," and the class that implements it acts as the "child.

- This principle ensures that inheritance hierarchies are well-designed and that 
subclasses adhere to the contracts defined by their superclasses.

- Violations of the LSP can lead to unexpected behavior or errors when substituting objects, 
making code harder to reason about and maintain.