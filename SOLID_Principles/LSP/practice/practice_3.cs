using System;

public class ElectronicDevice : IModifyPower, IModifyVolume
{
    public virtual void TurnOn()
    {
        Console.WriteLine("Device power state is now ON.");
    }

    public virtual void SetVolume(int level)
    {
        Console.WriteLine($"Volume adjusted to {level}.");
    }
}

public interface IModifyPower
{
    void TurnOn();
}

public interface IModifyVolume
{
    void SetVolume(int level);
}

public class SmartTelevision : IModifyPower, IModifyVolume
{
    public void TurnOn()
    {
        Console.WriteLine("TV screen is glowing.");
    }

    public void SetVolume(int level)
    {
        Console.WriteLine($"TV audio amplifier set to {level}.");
    }
}

public class SmartCeilingFan : IModifyPower
{
    public void TurnOn()
    {
        Console.WriteLine("Fan blades are spinning.");
    }
}

public class RoomController 
{
    public void ConfigureRoom(IModifyPower power, IModifyVolume volume)
    {
        power.TurnOn();
        volume.SetVolume(15);
    }
}

/*
var tv = new SmartTelevision();
var fan = new SmartCeilingFan();
var controller = new RoomController();

// This is now 100% type-safe and substitution-safe!
controller.ConfigureRoom(fan, tv); 

Liskov's principle applies to interfaces exactly the same way it applies to classes. An interface acts 
as the "parent contract," and the class that implements it acts as the "child.
*/

