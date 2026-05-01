Code smells - Violations:

1. OCP - if/else checks inside ProcessSubscription

2. DIP - instantiating a class with new keyword

3. SRP - ProcessSubscription class should only handle updating db and connection string should be inside its own method

4. ISP - throwing error for unused method

fixes: 

1. I can create an interface with a method called activatePlan, create new classes for Premium and Basic. Implement that interface on the new classes.

2. we can create a parent class/high level module for PremiumService and BasicService. This parent class can implement ISubscriptionService and IAutoRenewal, add private fields for PremiumService and BasicService and inject those fields inside the parent classes contructor to be called at runtime.

3. that if/else logic will be handled with OCP fix. Create DBContext class for db connection.

4. create separate interface for SetAutoRenewal method, call it IAutoRenewal because ISubscriptionService is too fat.


Refinement on your Fixes:
1. DIP Fix (Critical Adjustment): You suggested creating a "parent class" to inject Premium and Basic. In C#, the standard way to fix this is to inject the abstraction (ISubscriptionService) into the UserSubscriptionManager.
Interview Tip: Mention using a Factory or Dependency Injection Container to resolve which service to use based on the planType.
2. ISP/LSP Connection: You correctly spotted that BasicService throwing an error is an ISP violation. Note that this also violates LSP (Liskov Substitution) because BasicService cannot be used in place of ISubscriptionService without potentially crashing the app.
3. SRP Fix: Moving the connection string to a DbContext or Configuration file is exactly what a LexisNexis dev would expect.




1 May 2026:

You actually got a strong 4 out of 5 on the core principles. You successfully identified:
OCP (The if/else check) ✅
DIP (The new keyword) ✅
SRP (DB logic mixed with business logic) ✅
ISP (The fat interface/throwing errors) ✅
The only one you missed was LSP (Liskov Substitution), but that’s the hardest one! It’s the "hidden" violation that happens whenever you throw that NotImplementedException.
Where your "Fixes" can be polished:
In an intermediate interview, the "How to fix it" is as important as the "What is wrong."
Your DIP Fix: You suggested a "parent class" to inject concrete services. At LexisNexis, they will want to hear: "I will inject the interface into the constructor and let the DI Container handle the concrete class." Avoid using parent classes for this; use interfaces.
Your SRP Fix: You were 100% right about moving the DB logic. Just remember to use the word "Repository" or "DbContext"—those are the buzzwords they love.
The "Scorecard"
Identification: 85% (Excellent)
Proposed Architecture: 70% (Good, just need to lean more on Interfaces and Constructor Injection rather than inheritance).
