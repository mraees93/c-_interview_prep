public interface IRepository { void Save(string data); }

// 1. The Target Object
public class SqlRepository : IRepository 
{
    public void Save(string data) => Console.WriteLine($"Saving {data} to SQL.");
}

// 2. The Wrapper (Decorator) sharing the same interface
public class LoggingRepositoryDecorator : IRepository
{
    private readonly IRepository _innerRepository; // Wraps the target object

    public LoggingRepositoryDecorator(IRepository innerRepository)
    {
        _innerRepository = innerRepository;
    }

    public void Save(string data)
    {
        Console.WriteLine("LOG: Starting save operation..."); // Extra functionality
        _innerRepository.Save(data);                         // Delegates to target
        Console.WriteLine("LOG: Save operation completed.");   // Extra functionality
    }
}
/* 
Existing code uses the raw repository:

IRepository repo = new SqlRepository();

Without breaking any code, you can wrap it to inject new logic:

IRepository decoratedRepo = new LoggingRepositoryDecorator(repo);
decoratedRepo.Save("User_Profile_Data"); 
*/
