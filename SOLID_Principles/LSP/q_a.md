1. How do you define LSP without using the academic definition?

LSP means that a program should still work correctly if you replace an instance of a parent class with an instance of any of its child classes. The child must honor the "contract" of the parent. If the parent promises a certain behavior, the child cannot break that promise (e.g., by throwing an unexpected error or returning a value outside the expected range).

Pro tip: Use the phrase "Behavioral Subtyping." It shows you understand that inheritance isn't just about sharing code; it’s about sharing behavior


2. Why is throwing a NotImplementedException in a subclass a violation of LSP?

If a base class has a method Save(), any code calling that method expects it to save data. If a subclass overrides Save() but simply throws a NotImplementedException, any calling code that was written to handle the base class will crash. You have broken the "substitutability" because the child cannot do what the parent promised.

Pro Tip: Mention that this is a sign you should probably use Interface Segregation (ISP) instead of forced inheritance


3. The 'Square-Rectangle' problem is a classic LSP violation. How would you solve it in C#?

The violation happens because a Square "is-a" Rectangle mathematically, but not behaviorally (changing a Square's width changes its height). To fix it, you should avoid the inheritance relationship entirely. Instead, have both Square and Rectangle implement a more general interface like IShape or IPolygon.

Pro tip: This shows you know when not to use inheritance—a key sign of an Intermediate dev.


4. How does LSP relate to 'Design by Contract'?

LSP is essentially the enforcement of contracts. A subclass must:
- Not have stricter preconditions (e.g., the parent accepts any integer, but the child only accepts positives).
- Not have weaker postconditions (e.g., the parent guarantees a non-null return, but the child returns null).

Pro Tip: If the child changes the "rules" of the method, it's no longer a valid substitute for the parent.


5. What are the common 'code smells' that suggest an LSP violation?

Using is or as keywords to check the specific type of a subclass before calling a method (e.g., if (bird is Penguin)).
Empty or "No-Op" method overrides.
Methods that return "Magic Values" to indicate they can't perform an action (e.g., returning -1 for a method that usually returns a positive ID).

pro tip: Explain that these checks make code brittle. If you add a 10th subclass, you’d have to find every if/else check in the system to update it.


LSP golden rule:

If you find yourself writing if (obj is SubclassA) in your main logic, you are likely violating LSP. The calling code should be "blind" to which specific subclass it is using. 