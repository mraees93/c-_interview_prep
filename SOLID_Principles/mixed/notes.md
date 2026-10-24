prac 1-4, 6, 7, 8, 9, 10 is only OCP, LSP, DIP violations

prac_9 - abstract class solutions with only OCP, LSP, DIP violations

prac 5 - LSP, ISP, SRP




To remember:

prac_8.cs:

Because the individual validator strategy classes (ConstitutionalAct, CriminalProcedureAct) now capture and hold their own required database repository instance via constructor injection, the high-level CitationVerificationEngine does not have to inject ISqlRepository at all.

"If the engine doesn't pass the repository down, how do those strategy classes get their database instance?"

In a real standard modern .NET Core / .NET 8 Web API, the built-in dependency injection container handles this layering automatically behind the scenes. Your engine and strategies are registered inside your startup bootstrap file (Program.cs) like this:

// The runtime automatically intercepts this, sees ConstitutionalAct needs ISqlRepository,
// builds the SqlServerPrecedentCluster instance first, and injects it seamlessly!
builder.Services.AddScoped<ISqlRepository, SqlServerPrecedentCluster>();
builder.Services.AddScoped<ICitationsValidator, ConstitutionalAct>();
builder.Services.AddScoped<CitationVerificationEngine>();
