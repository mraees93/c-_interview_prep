here are the three core "side effects" of violating DIP. You can remember them by the acronym R.F.I. (Rigidity, Fragility, Immobility):
1. Rigidity (The "Ripple Effect")
When high-level logic is glued to a concrete class, any small change in the "detail" forces a recompile and redeploy of the entire service.
The Impact: It makes the system stiff. You can’t change the database or a third-party API without touching (and potentially breaking) your core business logic.
2. Fragility (The "Hidden Side Effects")
Violating DIP often results in the parent class losing control over the Life Cycle of its dependencies.
The Impact: If the low-level class has a bug (like a memory leak or an unclosed database connection), it "infects" the high-level class. Because the dependency is hard-coded, you can’t easily swap it out or isolate it to fix the issue, leading to bugs that are hard to trace.
3. Immobility (The "Anti-Reuse" Problem)
Your business logic becomes un-portable because it is physically tied to specific infrastructure (like a SQL library or a specific Windows service).
The Impact: You cannot reuse your valuable business rules in a different project (e.g., moving from a Web App to a Mobile App) because you’d have to drag along the entire database layer and all its heavy dependencies.
Interview Cheat Sheet:
If the interviewer asks why DIP matters, just say: "Without it, our core business logic becomes rigid to change, fragile to bugs in low-level details, and impossible to reuse across different environments."