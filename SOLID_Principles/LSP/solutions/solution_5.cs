public interface ILoungeAccess
{
    void GrantLoungeAccess();
}

public interface ITrainer
{
    void BookPersonalTrainer();
}

public abstract class GymMembership : ITrainer
{
    public string? HolderName { get; set; }

    public abstract void BookPersonalTrainer();
}

public class PremiumMembership : GymMembership, ILoungeAccess
{
    public override void BookPersonalTrainer()
    {
        Console.WriteLine($"Personal trainer session successfully reserved for {HolderName}.");
    }

    public void GrantLoungeAccess()
    {
        Console.WriteLine($"Premium VIP Lounge doors unlocked for {HolderName}.");
    }
}

public class BasicMembership : GymMembership
{
    public override void BookPersonalTrainer()
    {
        Console.WriteLine($"Personal trainer session successfully reserved for {HolderName}.");
    }
}

public class CheckInCounter
{
    public void PrepareGymVisit(GymMembership membership)
    {
        membership.BookPersonalTrainer();

        if (membership is ILoungeAccess vipAccess)
        {
            vipAccess.GrantLoungeAccess();
        }
    }
}
