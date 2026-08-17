namespace OOP_Basics.practice;

/*
1. The Bad Way: Class Inheritance (Is-A Relationship)This design forces a rigid coupling. If you modify the base class, you risk breaking all 
    derived subclasses (The Fragile Base Class problem).
*/

public class LoggableDocument
{
    public void LogToDisk(string message)
    {
        // Tight coupling to local file system
        System.IO.File.AppendAllText("app.log", message);
    }
}

// Every time you want logging capability, you must inherit from this class.
// What happens if LegalBrief needs to inherit from a "DatabaseEntity" base class later? 
// C# only allows single inheritance, so you are structurally stuck.
public class LegalBrief2 : LoggableDocument
{
    public void ProcessCase()
    {
        LogToDisk("Processing brief metadata...");
        // Core business logic here
    }
}


/*
2. The Good Way: Object Composition (Has-A Relationship)This design relies on combining independent behaviors via abstractions. It keeps your class 
    lightweight, flexible, and completely unit-testable.
    Testing Flex: With inheritance, testing LegalBrief forces a dependency on the real file system. With composition, you can easily pass a Mock<ILogger> 
    in your unit tests.
    Dynamic Swapping: If a requirement changes and you need to log to an cloud monitoring cluster instead of a local disk, you simply swap the injected 
    implementation without modifying a single line of code inside LegalBrief.
*/

public interface ILogger
{
    void Log(string message);
}

public class LegalBrief3
{
    private readonly ILogger _logger;

    public LegalBrief3(ILogger logger)
    {
        _logger = logger;
    }

    public void ProcessCase()
    {
        _logger.Log("object composition favored over inheritance");
    }
}

