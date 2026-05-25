//Use this to isolate your structural framework (Entity Framework Core) from your application logic, 
//making unit tests incredibly easy to write.


// Simple model representing a domain entity.
public class User { public int Id { get; set; } public string Name { get; set; } = string.Empty; }

// INTERVIEW KEY: The Repository Abstraction.
// The domain layers only call these interface methods. They are shielded from underlying database types.
public interface IUserRepository
{
    User GetById(int id);
}

// INTERVIEW KEY: Production Implementation.
// This handles heavy, live infrastructure details. It links directly to SQL databases via an ORM.
public class SqlUserRepository : IUserRepository
{
    private readonly MyDbContext _context;

    // Injecting the database context layer directly here.
    public SqlUserRepository(MyDbContext context) => _context = context;

    // Handles the actual querying logic.
    public User GetById(int id) => _context.Users.Find(id) ?? throw new KeyNotFoundException();
}

// INTERVIEW KEY: Mock/Fake Implementation for Unit Testing.
// This lives in your Test Project. It satisfies the IUserRepository contract WITHOUT connecting 
// to a real database. It bypasses networks and disk I/O, allowing tests to run instantly.
public class FakeUserRepository : IUserRepository
{
    // Returns hardcoded objects straight from local system memory.
    public User GetById(int id) => new User { Id = id, Name = "Test User" };
}
