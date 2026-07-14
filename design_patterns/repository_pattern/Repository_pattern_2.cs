using System;
using System.Collections.Generic;
using System.Linq;

// 1. The Model
public record User(int Id, string Name);

// 2. The Abstraction
public interface IUserRepository
{
    User? Get(int id);
    void Add(User user);
}

// 3. The Minimal Implementation
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public User? Get(int id) => _users.FirstOrDefault(u => u.Id == id);
    public void Add(User user) => _users.Add(user);
}

// 4. Execution Entry Point
public class Program
{
    public static void Main()
    {
        // Instantiate the repository implementation through its abstraction
        IUserRepository repo = new InMemoryUserRepository();

        repo.Add(new User(1, "Alice"));
        repo.Add(new User(2, "Bob"));

        User? foundUser = repo.Get(1);
        Console.WriteLine($"Found User: {foundUser?.Name}"); // Output: Alice
    }
}
