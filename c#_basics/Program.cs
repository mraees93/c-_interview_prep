public struct Point //struct is value type
{
    public int X;
    public int Y;
}
public class Customer //class is reference type
{
    public string Name { get; set; }
}
public interface ILogger
{
    void Log(string message);
}
public class FileLogger : ILogger
{
    void ILogger.Log(string message)
    {
        Console.Write(message);
    }
}
public delegate void DisplayDelegate();

public class Program
{
    // public static void UpdatePoint(Point p)
    // {
    //     p.X = 100;
    // }

    // public static string WhatsYourName(string name)
    // {
    //     string greeting = "Hello "; //strings are immutable
    //     greeting += name;
    //     return greeting;
    //     //The variable greeting drops its old reference and points to this new address. The old "Hello "
    //     // string floats in memory until the Garbage Collector cleans it up later.
    // }

    public static void SayHi() { Console.Write("Hi "); }
    public static void SayBye() { Console.Write("Bye"); }

    public static void Main()
    {
        // Point myPoint = new Point();
        // myPoint.X = 10;
        // myPoint.Y = 20;
        // UpdatePoint(myPoint);
        // Console.WriteLine(myPoint.X); //10 because UpdatePoint updates a duplicate copy

        //Console.WriteLine(WhatsYourName("raees"));

        // Customer c1 = new Customer { Name = "Alice" };//customer object is created in heap memory, c1 holds a pointer (reference address) to that spot
        // Customer c2 = c1;//not copying the object. Only copying the pointer. c1,c2 point to the exact same customer object in memory
        // c2.Name = "Bob";//modifying the shared object
        // Console.WriteLine(c1.Name);

        // FileLogger logger = new FileLogger();
        // ((ILogger)logger).Log("Logging"); //You must explicitly cast the object to the interface type first
        // Console.WriteLine(logger);

        DisplayDelegate del = SayHi;
        del += SayBye; //array queue holding [SayHi, SayBye]
        del -= SayHi; //unlinks and leaves only [SayBye]
        del();
    }
}
