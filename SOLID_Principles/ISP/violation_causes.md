When you violate the Interface Segregation Principle, you aren't just making the code "untidy"—you are creating technical debt that directly impacts the build and deployment process.
Here are the 3 core consequences of an ISP violation for your LexisNexis interview:
1. The "Recompile and Redeploy" Chain Reaction
When an interface is "fat" (has too many unrelated methods), a change to one method signature forces every class implementing that interface to be recompiled, even if they don't use the method that changed.
The Impact: This creates massive "bloat" in your CI/CD pipeline. A minor change in an Admin feature could trigger a full rebuild and redeploy of Guest and Auditor modules.
2. Forced "Polluted" Implementations
Developers are forced to write "dummy code" to satisfy the compiler for methods the class shouldn't have.
The Impact: This leads to a codebase filled with NotImplementedException or empty void methods. It hides the true intent of the class, making it impossible for a new developer to tell at a glance what the class is actually capable of doing.
3. Violation of the "Principle of Least Privilege"
ISP violation creates a security and stability risk by exposing functionality to parts of the system that shouldn't have access to it.
The Impact: If a GuestUser class implements a "fat" interface that includes DeleteRecord(), a bug or a malicious actor could theoretically call that method. By segregating interfaces, you ensure that a class literally cannot perform actions it isn't authorized for.

Quick Memorization Recap:
Bloated Builds: Too many unnecessary recompilations.
Lying Code: Classes full of NotImplemented "junk."
Security Risk: Exposing dangerous methods to unauthorized classes.