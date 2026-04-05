using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//A class inherits from another child class, forming a chain.
//A Smartphone is a type of Phone, and a Phone is a type of ElectronicDevice.

namespace OOP_Basics.inheritance.multi_level_inheritance
{
    public class ElectronicDevice
    {
        public void PowerOn() => Console.WriteLine("Device is powered on.");
    }

    class Phone : ElectronicDevice
    {
        public void MakeCall() => Console.WriteLine("Calling...");
    }

    class Smartphone : Phone
    {
        public void BrowseInternet() => Console.WriteLine("Browsing Internet...");
    }

    class Program
    {
        static void Main()
        {
            Smartphone myPhone = new Smartphone();
            myPhone.PowerOn();
            myPhone.MakeCall();
            myPhone.BrowseInternet();
        }
    }
}