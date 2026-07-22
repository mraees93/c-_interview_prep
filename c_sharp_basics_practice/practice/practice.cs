namespace c_sharp_basics_practice.practice;

public interface ILogger
{
    void Log(string message, string level = "Info");
}

public class ConsoleLogger : ILogger
{
    public void Log(string message, string level = "Info")
    {
        System.Console.WriteLine($"[{level}] {message}");
    }
}