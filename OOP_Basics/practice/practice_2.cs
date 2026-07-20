namespace OOP_Basics.practice;

public class LegalEngine
{
    public virtual void Process() => Console.WriteLine("Base Engine");
    public void Run() => Console.WriteLine("Base Run");
}

public class SpecializedSearchEngine : LegalEngine
{
    public override void Process() => Console.WriteLine("Specialized Engine");
    public new void Run() => Console.WriteLine("Specialized Run");
}