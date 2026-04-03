using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.ISP.violating_ISP
{
    public interface IShape
    {
        double Area();
        double Volume(); // problem: 2D shapes don't have volume!
    }
}