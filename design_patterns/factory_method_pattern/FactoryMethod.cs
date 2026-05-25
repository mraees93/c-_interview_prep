//Use this when your application cannot anticipate the exact concrete type of class it needs to 
//construct until runtime (e.g., matching a runtime string or database config switch).

// Common notification interface.
public interface INotifier { void Send(string message); }

// Concrete application variants.
public class SmsNotifier : INotifier { public void Send(string m) => Console.WriteLine($"SMS: {m}"); }
public class EmailNotifier : INotifier { public void Send(string m) => Console.WriteLine($"Email: {m}"); }

// INTERVIEW KEY: The Factory.
// This class isolates and centralises object creation logic. It shields client applications 
// from using the 'new' keyword on concrete instances, keeping software architectures cleanly decoupled.
public class NotifierFactory
{
    // The consumer calls this method passing a dynamic parameter (like an API setting or database variable).
    public INotifier CreateNotifier(string providerType)
    {
        // Centralised instantiation point. If a new type is created, you ONLY modify this switch expression.
        return providerType.ToLower() switch
        {
            "sms" => new SmsNotifier(),       // Instantiate SMS variant
            "email" => new EmailNotifier(),   // Instantiate Email variant
            // Safeguard against invalid runtime string arguments
            _ => throw new ArgumentException($"Unsupported provider type: {providerType}")
        };
    }
}
