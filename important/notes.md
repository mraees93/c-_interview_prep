Lifecycle modifiers:

Transient: Disposable water cup - A new instance is created everytime its requested by a class constructor.

Scoped: Jug of water - A single instance is created per HTTP web request, its thrown away by DI container when request ends. Used for short lived http requests e.g DbContext connections

We cant inject a short-lived service inside constructor of a long-lived Singleton service - this is the captive dependency trap - poisoning the geyser with a contaminated jug of water forever. 
Use an IScopeFactory if you wanna add a transient or scoped modifier inside a singleton

Singleton: Passage geyser heater - One instance created on startup. Long lived for the entirety of the app e.g Redis cache, Serilog loggers.


A singleton class MUST be thread-safe but it can either be stateful(mutable) or stateless(immutable)


What's Dependency injection?

Dependency injection is when passing external dependencies into a class constructor, rather than letting the class instantiate it internally

Why use it?

Satifies DIP by decoupling business logic from concrete implementations

It allows us to swap dependencies for mock objects during unit testing

It hands off object lifetime-management (T, S, S) to the DI container which prevents resource leaks on managed heap.