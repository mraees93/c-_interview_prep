1. How do you identify an SRP violation in an existing class without looking at the code?

Look at the dependencies in the constructor and the method names. If a UserService constructor requires a DatabaseContext, an SmtpClient, and a FileLogger, it’s doing too much. Similarly, if the method names are diverse (e.g., RegisterUser(), ExportToExcel(), and SendResetPasswordEmail()), the class has multiple reasons to change. A change in the Excel formatting library shouldn't risk breaking the User Registration logic.

Pro Tip: lots of if/else logic for different tasks is often a symptom of SRP failure


2. In an Angular context, how does SRP apply to Components vs. Services?

 A Component’s single responsibility is UI logic and data presentation. It should not handle data fetching, complex validation, or business rules directly. Those responsibilities belong in a Service. If a component has 500 lines of code handling API calls and data transformation, it violates SRP. By moving that logic to a service, the component only changes if the view changes, while the service only changes if the data source or business logic changes.

Pro Tip: Mention "Thin Components, Fat Services" as a design goal.


3. Does SRP mean a class should only have one single method?
No. "Responsibility" is not the same as "function." A responsibility is a cohesive set of behaviors tied to a single actor or business requirement. For example, a ReportGenerator class might have methods like FormatHeader(), ParseData(), and RenderFooter(). While these are different functions, they all serve the single responsibility of generating a report. If you added SaveToDatabase() to that class, that would be a second responsibility.