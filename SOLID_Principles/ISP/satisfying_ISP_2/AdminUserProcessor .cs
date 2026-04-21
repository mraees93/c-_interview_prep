using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.ISP.satisfying_ISP_2
{
    public interface IReadable
    {
        void Read();
    }

    public interface IModifiable
    {
        void Update();
        void Delete();
    }

    public class GuestUserProcessor : IReadable
    {
        public void Read() => Console.WriteLine("Reading...");
    }
    public class AdminUserProcessor : IReadable, IModifiable
    {
        public void Read() => Console.WriteLine("Reading...");
        public void Update() => Console.WriteLine("Updating...");
        public void Delete() => Console.WriteLine("Deleting...");
    }
}