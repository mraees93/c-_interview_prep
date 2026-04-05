using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.satisfying_DIP
{
    public class Car
    {
        private IEngine engine;

        public Car(IEngine engine)
        {
            this.engine = engine;
        }

        public void StartCar()
        {
            engine.Start();
            System.Console.WriteLine("Car started.");
        }
    }
}