Lifecycle modifiers:

Transient: Disposable water cup - A new instance is created everytime its requested by a class constructor.

Scoped: Jug of water - A single instance is created per HTTP web request, its thrown away by DI container when request ends. Used for short lived http requests e.g DbContext connections

We cant inject a short-lived service inside coonstructor of a long-lived Singleton service - this is the captive dependency trap - poisoning the geyser with a contaminated jug of water forever. 
Use an IScopeFactory if you wanna add a transient or scoped modifier inside a singleton

Singleton: Passage geyser heater - One instance created on startup. Long lived for the entirety of the app e.g Redis cache, Serilog loggers.


A singleton class MUST be thread-safe but it can either be stateful(mutable) or stateless(immutable)