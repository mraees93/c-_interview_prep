using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Basics.abstraction
{
    abstract class Vehicle
    {
        //abstract method (must be implemented by derived classes)
        public abstract void Start();

        //concrete method (already implemented)
        public void Stop()
        {
            Console.WriteLine("Vehicle stopped.");
        }
    }

//car class inherits from vehicle
class Car : Vehicle
{
    public override void Start()
    {
        Console.WriteLine("Car is starting.");
    }
}

//bike class inherits from vehicle
class Bike : Vehicle
{
    public override void Start()
    {
        Console.WriteLine("Bike is starting.");
    }
}

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