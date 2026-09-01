namespace OOP_Basics.practice;

public class ParentDocument
{
    public ParentDocument()
    {
        Console.WriteLine("Parent Constructor");
        //Render();
    }

    public virtual void Render() => Console.WriteLine("Parent Rendering");
}

public class LegalBrief : ParentDocument
{
    private string _briefType = "Statute";

    public LegalBrief()
    {
        Console.WriteLine("Child Constructor");
        _briefType = "CaseLaw";
    }

    public override void Render() => Console.WriteLine($"Child Rendering: {_briefType}");
}

/*By calling Render() on the instance after instantiation, you guarantee the object lifecycle executes in a 
safe, predictable sequence:
1. Inline Fields Initialize: _briefType is safely set to "Statute"
2. Parent Constructor Completes: ParentDocument() executes and logs its message without triggering any 
    premature polymorphic side effects.
3. Child Constructor Completes: LegalBrief() executes safely and updates _briefType to "CaseLaw".
4. The Object Is Now Stable: Only now do you invoke doc.Render()
*/