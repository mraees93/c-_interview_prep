//Use this inside Program.cs to explicitly tell the .NET runtime how to manage object creation and memory cleanup.

/*
"I implement Dependency Injection using Constructor Injection, passing the abstraction interface directly into the class constructor. 
I then configure the Service Lifetime in Program.cs to control whether the .NET runtime should instantiate a fresh object every time, 
reuse it within an HTTP request, or maintain a single instance globally."
*/

var builder = WebApplication.CreateBuilder(args);

// ====================================================================================
// 1. TRANSIENT LIFETIME
// INTERVIEW KEY: "Fresh instance every time."
// Whenever an architectural layer requests this interface, .NET instantiates a brand new object.
// Best Use Case: Lightweight, completely stateless services that do not hold temporary data.
// ====================================================================================
builder.Services.AddTransient<ITransientService, TransientService>();

// ====================================================================================
// 2. SCOPED LIFETIME
// INTERVIEW KEY: "One instance per HTTP Request."
// Reused across the entire web request pipeline thread. Disposed automatically when the request ends.
// Best Use Case: Entity Framework DbContext, or stateful services bound to a specific user session.
// DANGER: Injecting a Scoped service directly into a Singleton will cause memory leaks or deadlocks!
// ====================================================================================
builder.Services.AddScoped<IScopedService, ScopedService>();

// ====================================================================================
// 3. SINGLETON LIFETIME
// INTERVIEW KEY: "One single instance forever."
// Instantiated exactly once when the application spins up, and shared globally across all threads.
// Best Use Case: Memory caches, configuration managers, or expensive third-party clients.
// CRITICAL: Ensure Singleton implementations are thread-safe, as multiple users access them concurrently.
// ====================================================================================
builder.Services.AddSingleton<ISingletonService, SingletonService>();

var app = builder.Build();
