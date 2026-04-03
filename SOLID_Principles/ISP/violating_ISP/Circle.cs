using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.ISP.violating_ISP
{
    public class Circle : IShape
    {
        public double Radius { get; set; }

        public double Area()
        {
            return Math.PI * Math.Pow(Radius, 2);
        }

        public double Volume()
        {
            throw new InvalidOperationException("Volume not applicable for 2D shapes.");
        }
    }
}