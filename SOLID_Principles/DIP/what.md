High level classes shouldnt depend on low level classes. They should depend on abstraction

Abstractions are abstract classes and interfaces.

Dependency inversion is the strategy of depending on abstract classes and interfaces rather than concreate classes. 

To satisfy DIP, I move the dependency out of the high-level module. Instead of the high-level class 'newing up' a specific low-level tool, I define an Interface (the contract). I then Inject that interface into the high-level class's constructor.
This creates a buffer zone. Now, I can change or swap the low-level implementation (e.g., switching from SQL to a Web API) without needing to modify or re-test the high-level business logic. The two modules are no longer 'glued' together; they are both connected to a stable abstraction.

If you want to prove you've mastered DIP, mention these three things together:
Abstractions: High-level and low-level modules both depend on interfaces.
DI Container: You use .NET Core’s built-in IServiceCollection to manage these.
Mocking: You use libraries like Moq or NSubstitute in your Unit Tests to take advantage of these inverted dependencies.