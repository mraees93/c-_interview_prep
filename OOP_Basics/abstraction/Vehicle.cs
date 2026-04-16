using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Basics.abstraction
{
    // abstract class Vehicle
    // {
    //     //abstract method (must be implemented by derived classes, you cannot create objects from it)
    //     public abstract void Start();

    //     //concrete method (already implemented)
    //     public void Stop()
    //     {
    //         Console.WriteLine("Vehicle stopped.");
    //     }
    // }

    //An interface is a completely "abstract class", which can only contain abstract methods and properties (with empty bodies)
    interface IVehicle
    {
        void Start();
        void Stop();
    }

//car class inherits from vehicle
class Car : IVehicle
{
    public void Start()
    {
        Console.WriteLine("Car is starting.");
    }
    //Abstract methods are "incomplete" and act as a requirement for child classes.

    // Using override tells the compiler that this method is intentionally fulfilling that requirement, rather than 
    // just being a new method with the same name.

     public void Stop()
    {
        Console.WriteLine("Car is stopping.");
    }
}

//bike class inherits from vehicle
class Bike : IVehicle
{
    public void Start()
    {
        Console.WriteLine("Bike is starting.");
    }

    public void Stop()
    {
        Console.WriteLine("Bike is stopping.");
    }
}

/*
Abstraction is the act of hiding these internal details. A user of your code only needs to know about IVehicle; 
they don't need to care how the bike’s engine or chain works internally.

Implementation Hiding
By using the interface, you can write code that operates on a vehicle without knowing it’s a Bike
*/

// class Program
// {
//     static void Main()
//     {
//         Vehicle myCar = new Car();
//         myCar.Start();
//         myCar.Stop();

//         Vehicle myBike = new Bike();
//         myBike.Start();
//         myBike.Stop();
//     }
// }
}