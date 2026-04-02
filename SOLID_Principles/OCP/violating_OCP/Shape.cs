using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.OCP.violating_OCP
{
    public class Shape
    {
        public ShapeType Type { get; set; }
        public double Radius { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }

        public double CalculateArea()
        {
            switch (Type)
            {
                case ShapeType.Circle:
                    return Math.PI * Math.Pow(Radius, 2);
                case ShapeType.Rectangle:
                    return Length * Width;
                default:
                    throw new InvalidOperationException("Unsupported shape type");
            }
        }
    }
}

/*
 the Shape class has a method, CalculateArea(), that calculates the area 
 based on the type of shape. Adding a new shape, such as a triangle, 
 would require modifying the existing Shape class, violating the OCP

 To adhere to the Open/Closed Principle, we should design the system in a 
 way that allows for extension without modification.
*/