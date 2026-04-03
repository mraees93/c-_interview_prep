using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.LSP.satisfying_LSP
{
    public class Square : Shape
    {
        private double sideLength;

        public double SideLength
        {
            get => sideLength;
            set
            {
                sideLength = value;
            }
        }

        public override double Area => sideLength * sideLength;
    }
}