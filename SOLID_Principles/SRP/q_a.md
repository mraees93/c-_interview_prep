1. How do you identify an SRP violation in an existing class without looking at the code?

Look at the dependencies in the constructor and the method names. If a UserService constructor requires a DatabaseContext, an SmtpClient, and a FileLogger, it’s doing too much. Similarly, if the method names are diverse (e.g., RegisterUser(), ExportToExcel(), and SendResetPasswordEmail()), the class has multiple reasons to change. A change in the Excel formatting library shouldn't risk breaking the User Registration logic.

Pro Tip: lots of if/else logic for different tasks is often a symptom of SRP failure


2. In an Angular context, how does SRP apply to Components vs. Services?

 A Component’s single responsibility is UI logic and data presentation. It should not handle data fetching, complex validation, or business rules directly. Those responsibilities belong in a Service. If a component has 500 lines of code handling API calls and data transformation, it violates SRP. By moving that logic to a service, the component only changes if the view changes, while the service only changes if the data source or business logic changes.

Pro Tip: Mention "Thin Components, Fat Services" as a design goal.


3. Does SRP mean a class should only have one single method?

No. "Responsibility" is not the same as "function." A responsibility is a cohesive set of behaviors tied to a single actor or business requirement. For example, a ReportGenerator class might have methods like FormatHeader(), ParseData(), and RenderFooter(). While these are different functions, they all serve the single responsibility of generating a report. If you added SaveToDatabase() to that class, that would be a second responsibility.

Pro Tip: Explain that over-splitting (creating a class for every single method) can lead to "Shotgun Surgery," where a single small change forces you to edit dozens of tiny classes.


4. What is the relationship between SRP and 'High Cohesion' ? (high cohesion refers to the degree to which elements within a single module—such as a class, method, or namespace—are focused on a single, well-defined purpose) 

SRP is the path to achieving High Cohesion. Cohesion refers to how closely related the logic within a single module or class is. If a class follows SRP, its methods are all working toward the same goal, meaning it has high cohesion. If a class has low cohesion (doing many unrelated things), it is much harder to maintain, understand, and reuse.


5. How do you handle 'Cross-Cutting Concerns' like Logging or Validation without violating SRP ?

You use the Decorator Pattern or Aspect-Oriented Programming (AOP). If you put logging, caching, and validation inside your business logic method, that method now has four responsibilities. Instead, you should keep the business logic clean in its own class and "wrap" it with a LoggingDecorator or use Middleware (in .NET Core) to handle the logging. This keeps the core service focused on one thing.

LexisNexis Context: Since they use ASP.NET Core, mentioning Action Filters or Middleware as a way to handle validation and logging outside of your Controllers is a very "Intermediate-level" answer that shows you know the framework deeply.


Quick Check: Is it SRP?
If you can describe what a class does without using the word "and," it probably follows SRP.
Bad: "This class calculates the tax and saves it to the database."
Good: "This class calculates the tax."