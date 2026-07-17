public abstract class AnnualBonus
{
    public virtual void CalculateAnnualBonus()
    {
        System.Console.WriteLine("Bonus is...");
    }
}

public class Employee : AnnualBonus
{
    public override void CalculateAnnualBonus()
    {
        // Standard corporate bonus calculation logic
        System.Console.WriteLine(3 * 12);
    }
}

public class Contractor : AnnualBonus
{
    public override void CalculateAnnualBonus()
    {
        System.Console.WriteLine(2.25 * 8);
    }
}
