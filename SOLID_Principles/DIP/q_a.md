1. What is the 'Inversion' actually referring to in Dependency Inversion?

It refers to the direction of the dependency. In a normal flow, a High-Level Class (e.g., BillingService) points directly to a Low-Level Class (e.g., SqlDatabase). When you "Invert" it, both classes now point towards a central abstraction (the IDatabase interface). The dependency arrow literally flips direction, pointing toward the center of your architecture rather than toward the infrastructure.


2. Is it a violation of DIP to use the new keyword in a Service?

Yes, if you are new-ing up a concrete implementation of a dependency (like var repo = new SqlRepository()). This "hardcodes" the dependency, making it impossible to swap for a Mock during testing or to change the implementation later without modifying the service.

Exception: It is not a violation to use new for "Plain Old CLR Objects" (POCOs), DTOs, or Entities (like var user = new User()), as these are data containers, not logic-heavy dependencies.


3. How does DIP enable Parallel Development in a team?

Because DIP relies on interfaces, a Senior Dev can define the ILegalSearchProvider interface on day one. One developer can then work on the Angular frontend using a "Mock" implementation, while another developer builds the actual ElasticSearch backend. Neither has to wait for the other to finish; they just both have to "agree" on the interface contract.

Pro Tip: This is very relevant at LexisNexis because they have multiple teams working on integrated legal products.

4. How do you handle a scenario where a third-party library doesn't provide an interface for its main class?

Use the Adapter Pattern or Wrapper Pattern. I would create my own interface (e.g., IThirdPartyWrapper) and a concrete class that implements that interface by calling the third-party library. My application then depends on my interface, keeping the third-party code isolated and replaceable.

pro tip: This shows you know how to protect the "Core" of your application from external changes—a sign of a mature Intermediate Engineer.

If you want to prove you've mastered DIP, mention these three things together:
Abstractions: High-level and low-level modules both depend on interfaces.
DI Container: You use .NET Core’s built-in IServiceCollection to manage these.
Mocking: You use libraries like Moq or NSubstitute in your Unit Tests to take advantage of these inverted dependencies.