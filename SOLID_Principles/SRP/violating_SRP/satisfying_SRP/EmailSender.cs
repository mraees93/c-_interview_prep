using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP.violating_SRP.satisfying_SRP
{
    public class EmailSender
    {
        public void SendEmail(string message, string recipient)
        {
            Console.WriteLine($"Sending email to {recipient}: {message}");
        }
    }
}