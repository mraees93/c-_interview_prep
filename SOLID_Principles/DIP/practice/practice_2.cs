public class LocalFileLogger : IFileLogger
{
    public void WriteLog(string text)
    {
        System.IO.File.AppendAllText("app.log", text);
    }
}

public interface IFileLogger
{
    void WriteLog(string text);
}

public class AuthenticationService
{
    private IFileLogger _logger;

    public AuthenticationService(IFileLogger logger)
    {
        _logger = logger;
    }

    public void Login(string username)
    {
        _logger.WriteLog($"User {username} logged in.");
    }
}
