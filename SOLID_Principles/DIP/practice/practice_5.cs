using System;

public class LocalFileLogger1 : ILogger
{
    public void LogInfo(string text)
    {
        Console.WriteLine($"[FILE LOG] {text}");
    }
}

public interface ILogger
{
    void LogInfo(string text);
}

interface IUserRegistration
{
    void RegisterUser(string username, string email);
}

public class UserRegistrationManager : IUserRegistration
{
    private ILogger _logger;

    public UserRegistrationManager(ILogger logger)
    {
        _logger = logger;
    }

    public void RegisterUser(string username, string email)
    {
        _logger.LogInfo($"Registering user: {username}");
        Console.WriteLine($"User {username} successfully registered with email {email}.");
        _logger.LogInfo($"Registration finalized for: {username}");
    }
}
