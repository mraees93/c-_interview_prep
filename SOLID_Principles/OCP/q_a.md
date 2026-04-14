1. How do you explain OCP to a junior developer using a real-world analogy?

Think of a wall power outlet. It is closed for modification (you don’t tear the wall open to change the wiring every time you buy a new device), but it is open for extension (you can plug in a lamp, a toaster, or a phone charger because they all follow the same "interface" or plug standard). In code, we use interfaces so new features can "plug in" to the existing system.

Use the word "Pluggable Architecture." It shows you understand system design beyond just classes.


2. You have a ReportGenerator class with a large switch statement that checks ReportType. How does this violate OCP ?

It violates OCP because every time a new report type is added (e.g., "Tax Report," "Summary Report"), I have to modify the ReportGenerator class to add a new case. This risks breaking existing report logic. To fix this, I would create an IReport interface with a Generate() method. Each report type becomes its own class. The ReportGenerator then just calls report.Generate() without needing to know which specific type it is handling.

Pro Tip: Mention that this refactoring also helps with Unit Testing, as you can test each report class independently.


3. What is the relationship between OCP and the Strategy Pattern ?
Strategy Pattern is frequently used to replace large if-else or switch statements with a more flexible, object-oriented structure that adheres to the OCP.
The Strategy Pattern is one of the most common ways to implement OCP. It allows you to define a family of algorithms (strategies), encapsulate each one, and make them interchangeable. By injecting an interface into a class, you allow the behavior of that class to be extended at runtime by passing in different implementations, without ever changing the class’s internal code.

LexisNexis Tip: LexisNexis uses a lot of "Rules Engines" for legal data. The Strategy Pattern is perfect for applying different legal rules to the same dataset.

4. Can a class be 100% closed to all changes? Is that the goal ?

No. It is impossible and often impractical to make a class closed to every possible type of change. The goal is to be strategically closed. You should apply OCP to the areas of the application that are most likely to change or grow based on business requirements. Over-applying OCP can lead to "abstraction distraction," making the code harder to read.

Pro Tip: Use the phrase "Identify the axis of change." It shows you think about business requirements before over-engineering

5. How does Dependency Injection (DI) help in following the Open/Closed Principle ?
- Dependency Injection (DI) is a software design pattern used in C# and .NET to achieve Inversion of Control (IoC). Instead of a class creating its own dependencies (e.g., using the new keyword), the dependencies are "injected" into the class from an external source

DI allows us to swap implementations at the configuration level (e.g., in Program.cs or Startup.cs) rather than inside the business logic. If my service depends on IEmailProvider, I can swap SmtpEmailProvider for SendGridEmailProvider just by changing the DI container registration. The service itself remains closed because its code never changes, but its functionality is extended by the new provider.

i can also implement dependency injection by creating an abstract class or interface, implement it, create different classes to eliminate if/else statements and use polymorphism to change to form of the methods inside the abstract class or interface

Since LexisNexis uses .NET Core, mention how the built-in IServiceCollection makes OCP easier to implement.

Key Takeaway for OCP:
If you see an if-else or switch statement checking for "Types" or "Enums" to decide logic, it is almost always an OCP violation.