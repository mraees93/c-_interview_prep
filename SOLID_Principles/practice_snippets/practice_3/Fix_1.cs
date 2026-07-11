using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// LSP
namespace SOLID_Principles.practice_snippets.practice_3
{
    public class Fix_1
    {
        interface ITemperature
        {
            public void SetTemperature(double target);
        }

        interface ILight
        {
            bool IsOff { get; set; }
        }
        public class SmartHomeDevice : ITemperature
        {
            public virtual void SetTemperature(double target)
            {
                // Temperature logic
                System.Console.WriteLine($"target is now {target}");
            }
        }

        public class SmartLight : ILight
        {
            public bool IsOff { get; set; } = false;
        }

    }
}