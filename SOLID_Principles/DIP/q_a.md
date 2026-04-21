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

5. How would you explain DIP to a junior developer without using the word 'Interface'?

I’d explain it using the Plug-and-Socket analogy. Imagine a wall socket (the abstraction) and a lamp (the detail). The lamp doesn’t care how the electricity is generated—whether it's coal, nuclear, or solar—it just needs the plug to fit the socket. DIP is about ensuring our high-level business logic defines the 'socket' it needs, rather than being hard-wired directly into a specific power plant.

6. What is the difference between Dependency Inversion (DIP), Dependency Injection (DI), and Inversion of Control (IoC)?

DIP is the Principle (the 'What'): It’s the high-level rule that we should depend on abstractions.
IoC is the Design Pattern (the 'Where'): It’s the broad concept of handing over control of the flow to a framework.
DI is the Implementation (the 'How'): It’s the actual act of passing the dependency (usually via a constructor) into the class that needs it."

7. If we have 50 services and we create 50 interfaces just to satisfy DIP, aren't we over-engineering?

It can be. This is called 'Interface Explosion.' In an intermediate role, I aim for balance. If a class is a simple internal helper that will never be swapped and has no side effects (like I/O), an interface might be overkill. However, for any class that performs business logic or external communication, the 'cost' of the interface is worth the 'benefit' of testability and decoupling.

8. How does violating DIP affect your Unit Tests?

It makes true unit testing impossible. If my Service class creates a new SqlDatabase(), my test is forced to connect to a real database. This makes the tests slow, flaky, and dependent on the environment. By following DIP, I can inject a Mock or Stub implementation, allowing me to test my business logic in isolation without any external dependencies.