using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.ISP.violating_ISP_2
{
    public interface IDocumentProcessor
    {
        void Read();
        void Update();
        void Delete();
    }
    public class GuestUserProcessor : IDocumentProcessor
    {
        public void Read() => Console.WriteLine("Reading...");

        // Code smell: Forced to throw exceptions for methods it shouldn't have.
        public void Update() => throw new NotImplementedException();
        public void Delete() => throw new NotImplementedException();
    }
}