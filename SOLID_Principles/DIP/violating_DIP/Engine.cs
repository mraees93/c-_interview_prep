using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.violating_DIP
{
    public class Engine // Engine is our "low-level" module / child class
    {
        public void Start()
        {
            System.Console.WriteLine("Engine started.");
        }
    }
}