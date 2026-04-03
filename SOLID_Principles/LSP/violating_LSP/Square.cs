using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.LSP.violating_LSP
{
    public class Square : Rectangle
    {
        public override double Width
        {
            get => base.Width;
            set => base.Width = base.Height = value;
        }

        public override double Height
        {
            get => base.Height;
            set => base.Height = base.Width = value;
        }
    }
}