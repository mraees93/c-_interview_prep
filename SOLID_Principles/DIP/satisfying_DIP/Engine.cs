using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.satisfying_DIP
{
    public class Engine : IEngine
    {
        public void Start()
        {
            System.Console.WriteLine("Engine started.");
        }
    }
}