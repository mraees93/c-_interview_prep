using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Basics.polymorphism.method_overriding
{
    public class Boss
    {
        public void GetWealth()
        {
            Console.WriteLine("Owns a Mansion");
        }

        public virtual void Profession()
        {
            Console.WriteLine("Wants his son to be a Lawyer");
        }

        public virtual void Marriage()
        {
            Console.WriteLine("Marriage => Angelina");
        }
    }

    class Son : Boss
    {
        public override void Profession()
        {
            Console.WriteLine("Became a Software Engineer");
        }

        public override void Marriage()
        {
            Console.WriteLine("Marriage => Emma Watson");
        }
    }

    class Uncle {
    static void Main() {
        // Uncle talking to Boss
        Boss b = new Boss();
        b.GetWealth();  // Owns a Mansion
        b.Profession(); // Wants his son to be a Lawyer
        b.Marriage();   // Marriage => Angelina

        // Uncle talking to Son
        Son s = new Son();
        s.GetWealth();  // Owns a Mansion
        s.Profession(); // Became a Software Engineer
        s.Marriage();   // Marriage => Emma Watson

        // Uncle using dynamic method dispatch
        Boss b1 = new Son();
        b1.GetWealth();  // Owns a Mansion
        b1.Profession(); // Became a Software Engineer
        b1.Marriage();   // Marriage => Emma Watson

        // Not possible: Child reference and parent object
        // Son s1 = new Boss();  // Error
    }
}
}