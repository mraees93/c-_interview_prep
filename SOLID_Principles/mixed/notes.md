prac 1-4, 6, 7, 8, 9, 10, 13, 16, 19 - OCP, LSP, DIP violations with interface solutions

prac_9, 11, 14, 17 - abstract class solutions with only OCP, LSP, DIP violations

prac_19 onwards - LSP subtle behavioral violations




prac 5, 12, 15, 18 - LSP, ISP, SRP violations

prac_15 BillingEngine should be refactored like prac_18 DocumentVerificationEngine

pattern is called Interface Composition (often referred to as Interface Inheritance in C#).



1. Interface Segregation (Splitting)

First, you break a fat, monolithic contract down into tiny, role-focused interfaces (e.g., ILegalBillingPipeline and IInvoiceModifier).

2. Interface Composition (Combining)

Next, when a specific workflow requires multiple roles at once, you compose those small contracts together into a single, cohesive contract (public interface IInvoiceModifier : ILegalBillingPipeline).


Why the Panel Loves This Terminology

Using the exact term Interface Composition shows that you understand how to keep your system modular without creating messy method signatures that require three or four different interface parameters. You keep your interfaces small at the bottom, and compose them cleanly at the top where the orchestration engine needs them.

### 🚦 When to Use Which Approach

| Scenario | Recommended Pattern | Why? |
| :--- | :--- | :--- |
| **Roles are completely independent** (e.g., `IPrintable` and `IEmailSender`) | **Keep Separate** (Implement both on the class: `class Doc : IPrintable, IEmailSender`) | Prevents coupling unrelated capabilities together. |
| **Role B strictly requires Role A** (e.g., every modifiable document *must* also be verifiable) | **Interface Composition** (`interface IModifiable : IVerifiable`) | Keeps orchestration signatures clean and DRY without risking ISP violations. |

---


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
