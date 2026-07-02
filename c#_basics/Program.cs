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
public class Program
{
    // public static void UpdatePoint(Point p)
    // {
    //     p.X = 100; //this updates [SLOT B]
    // }

    // public static string WhatsYourName(string name)
    // {
    //     string greeting = "Hello "; //strings are immutable
    //     greeting += name;
    //     return greeting;
    //     //The variable greeting drops its old reference and points to this new address. The old "Hello "
    //     // string floats in memory until the Garbage Collector cleans it up later.
    // }

    //public delegate void DisplayDelegate();
    // public static void SayHi() { Console.Write("Hi "); }
    // public static void SayBye() { Console.Write("Bye"); }

    public static void Main()
    {
        // Point myPoint = new Point(); //Stack Memory [Slot A] holds myPoint (X = 10, Y = 20).
        // myPoint.X = 10;
        // myPoint.Y = 20;
        // UpdatePoint(myPoint); 
        // //C# looks at the data inside myPoint [Slot A], duplicates it,hands that duplicate to the method param variable p
        // Console.WriteLine(myPoint.X); //10 because UpdatePoint updates the duplicate copy [Slot B]

        //Console.WriteLine(WhatsYourName("raees"));

        // Customer c1 = new Customer { Name = "Alice" };//customer object is created in heap memory, c1 holds a pointer (reference address) to that spot
        // Customer c2 = c1;//not copying the object. Only copying the pointer. c1,c2 point to the exact same customer object in memory
        // c2.Name = "Bob";//modifying the shared object
        // Console.WriteLine(c1.Name);

        // FileLogger logger = new FileLogger();
        // ((ILogger)logger).Log("Logging"); //You must explicitly cast the object to the interface type first
        // Console.WriteLine(logger);

        // DisplayDelegate del = SayHi;
        // del += SayBye; //array queue holding [SayHi, SayBye]
        // del -= SayHi; //unlinks and leaves only [SayBye]
        // del();

        //Implicit Casting (automatically) - converting a smaller type to a larger type size: char -> int -> long -> float -> double
        // int myInt = 9;
        // double myDouble = myInt;       // Automatic casting: int to double

        // Console.WriteLine(myInt);    
        // Console.WriteLine(myDouble); 

        //Explicit Casting (manually) - converting a larger type to a smaller size type: double -> float -> long -> int -> char
        double myDouble = 9.78;
        int myInt = (int) myDouble;    // Manual casting: double to int

        Console.WriteLine(myDouble);   // Outputs 9.78
        Console.WriteLine(myInt);      // Outputs 9

        //Type conversion methods
        // int myInt = 10;
        // double myDouble = 5.25;
        // bool myBool = true;

        // Console.WriteLine(Convert.ToString(myInt));    
        // Console.WriteLine(Convert.ToDouble(myInt));    
        // Console.WriteLine(Convert.ToInt32(myDouble)); 
        // Console.WriteLine(Convert.ToString(myBool));
    }
}
