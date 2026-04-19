here are the three core indicators that a class is violating the Open/Closed Principle:
1. The "Switch-Statement" or "Enum" Trap
This is the most obvious sign. If you see a switch statement or a long chain of if-else blocks that check a "Type" or "Enum" to decide which logic to run, it's a violation.
The Cause: The logic for every possible variation is crammed into one method.
The Consequence: To add a new variation, you must modify the existing code, risking "regression bugs" (breaking old features while adding new ones).
2. Hard-coded Dependencies (The "New" Keyword)
If a class internally instantiates specific "helper" or "service" classes using new, it is locked into those specific behaviors.
The Cause: The class is responsible for both executing logic and deciding which implementation to use.
The Consequence: You cannot "extend" the class's behavior with a new implementation without opening the file and changing the new statement to a different class.
3. High "Change Frequency" (The OCP Litmus Test)
If a specific file in your project is modified every time a new business requirement or "plugin" is added, it is not "closed for modification."
The Cause: Lack of an abstraction layer (like an interface or abstract class) that allows new features to be added as separate, independent files.
The Consequence: This creates a "bottleneck" file where merge conflicts happen frequently, and testing becomes a nightmare because the "stable" logic is constantly being touched.

When the LexisNexis panel asks how to fix an OCP violation, use the phrase: "I would introduce a Strategy Pattern or an Abstraction." This shows you know exactly how to move the code from a "switch statement" to a "pluggable architecture."
