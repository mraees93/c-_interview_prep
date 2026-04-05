using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Basics.inheritance.single_inheritance
{
    public class Vehicle
    {
        public string Brand = "Toyota";
        public void Start() => Console.WriteLine("Vehicle is starting...");
    }

    class Car : Vehicle  // Child class
    {
        public string Model = "Corolla";
    }

    class Program
{
    static void Main()
    {
        Car myCar = new Car();
        Console.WriteLine($"Brand: {myCar.Brand}, Model: {myCar.Model}"); //car inherits brand from parent
        myCar.Start();
    }
}
}