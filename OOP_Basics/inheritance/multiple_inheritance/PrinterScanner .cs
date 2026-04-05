using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//C# does not support multiple inheritance with classes, but it allows multiple interfaces.
//A PrinterScanner is both a Printer and a Scanner.

namespace OOP_Basics.inheritance.multiple_inheritance
{
    interface IPrinter
    {
        void Print();
    }

    interface IScanner 
    {
        void Scan();
    }
    public class PrinterScanner : IPrinter, IScanner
    {
        public void Print() => Console.WriteLine("Printing document...");
        public void Scan() => Console.WriteLine("Scanning document...");
    }

    class Program
    {
        static void Main()
        {
            PrinterScanner device = new PrinterScanner();
            device.Print();
            device.Scan();
        }
    }
}