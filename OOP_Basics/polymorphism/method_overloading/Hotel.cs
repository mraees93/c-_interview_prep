using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Basics.polymorphism.method_overloading
{
    public class Hotel
    {
        public void GetRoom()
        {
            Console.WriteLine("Standard Room");
        }

        public void GetRoom(string type)
        {
            Console.WriteLine("Luxury Suite");
        }
    }

    class Guest
    {
        static void Main()
        {
            Hotel h = new Hotel();
            h.GetRoom(); // Standard Room
            h.GetRoom("Booked Luxury"); // Luxury Suite
        }
    }
}