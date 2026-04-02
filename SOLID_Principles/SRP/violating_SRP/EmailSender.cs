using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP.violating_SRP
{
    public class EmailSender
    {
        public void SendEmail(string message, string recipient)
        {
            // Email sending logic
            Console.WriteLine($"Sending email to {recipient}: {message}");
        }
    }
}