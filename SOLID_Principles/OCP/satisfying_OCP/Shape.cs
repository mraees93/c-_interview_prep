using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.OCP.satisfying_OCP
{
    public abstract class Shape
    {
        public abstract double CalculateArea();
    }

    public class Circle : Shape
    {
        public double Radius { get; set; }
        public override double CalculateArea()
        {
            return Math.PI * Math.Pow(Radius, 2);
        }
    }

    public class Rectangle : Shape
    {
        public double Length { get; set; }
        public double Width { get; set; }

        public override double CalculateArea()
        {
            return Length * Width;
        }
    }
}