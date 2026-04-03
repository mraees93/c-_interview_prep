using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.LSP.violating_LSP
{
    public class Rectangle : Shape
    {
        public virtual double Width { get; set; }

        public virtual double Height { get; set; }

        public override double Area => Width * Height;
    }
}