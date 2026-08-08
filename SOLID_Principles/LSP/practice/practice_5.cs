// using System;

// abstract class LoungeAccess
// {
//     public string? HolderName { get; set; }
//     public abstract void GrantLoungeAccess();
// }
// abstract class BasicAccess
// {
//     public string? HolderName { get; set; }
//     public abstract void GrantBasicAccess();
// }
// interface ITrainer
// {
//     void BookPersonalTrainer();
// }
//  class Membership : LoungeAccess, ITrainer
// {    
//     public override void GrantLoungeAccess()
//     {
//         Console.WriteLine($"Lounge access granted to {HolderName}.");
//     }

//     public void BookPersonalTrainer()
//     {
//         Console.WriteLine($"Personal trainer session successfully reserved for {HolderName}.");
//     }
// }

//  class PremiumMembership : LoungeAccess, ITrainer
// {
//     public override void GrantLoungeAccess()
//     {
//         Console.WriteLine($"Premium VIP Lounge doors unlocked for {HolderName}.");
//     }
//     public void BookPersonalTrainer()
//     {
//         Console.WriteLine($"Personal trainer session successfully reserved for {HolderName}.");
//     }
// }

//  class BasicMembership : BasicAccess, ITrainer
// {
//     public override void GrantBasicAccess()
//     {
//         System.Console.WriteLine($"Basic access granted for {HolderName}.");
//     }
//     public void BookPersonalTrainer()
//     {
//         Console.WriteLine($"Personal trainer session successfully reserved for {HolderName}.");
//     }
// }

// public class CheckInCounter
// {
//      void PrepareGymVisit(LoungeAccess loungeAccess, BasicAccess basicAccess, ITrainer trainer)
//     {
//         basicAccess.GrantBasicAccess();
//         trainer.BookPersonalTrainer();
//         loungeAccess.GrantLoungeAccess();
//     }
// }

using System;

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
