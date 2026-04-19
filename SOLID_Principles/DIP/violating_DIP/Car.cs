using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.violating_DIP
{
    public class Car //high level module / parent class
    {
        private readonly Engine engine;

        public Car() // constructor
        {
            this.engine = new Engine(); // Violates DIP: Direct dependency on concrete Engine class
            //It is a violation when you use new to instantiate service-like dependencies or "volatile" 
            //objects that perform logic, I/O, or database work.
        }

        public void StartCar()
        {
            engine.Start();
            System.Console.WriteLine("Car started.");
        }
    }
}