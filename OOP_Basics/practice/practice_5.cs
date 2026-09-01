public class BasePrinter
{
    public BasePrinter()
    {
        Console.WriteLine("BasePrinter Constructor Started.");
        
        // TRAP: Calling a virtual method inside the constructor
        InitPrinter(); 
    }

    public virtual void InitPrinter()
    {
        Console.WriteLine("Base initialization.");
    }
}

public class ChildPrinter : BasePrinter
{
    private List<string> _printQueue;

    public ChildPrinter()
    {
        // The child constructor body runs LAST
        _printQueue = new List<string>(); 
        Console.WriteLine("ChildPrinter Constructor Complete. Queue allocated.");
    }

    public override void InitPrinter()
    {
        Console.WriteLine("Child overriding initialization...");
        
        // CRASH: _printQueue is still null here!
        // Because BasePrinter's constructor invoked this method before 
        // ChildPrinter's constructor body could ever assign the new List().
        _printQueue.Add("Initialization Log"); 
    }
}