using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP
{
    public class AlertSystemExample
    {
        // 1. THE ABSTRACTION
        // This is the "bridge" that *DECOUPLES* the logic from the detail.
        public interface IMessageSender
        {
            void Send(string message);
        }

        // 2. LOW-LEVEL MODULES (Implementation Details)
        // These implement the abstraction. They are "pluggable."
        public class EmailService : IMessageSender
        {
            public void Send(string message) => Console.WriteLine($"Email sent: {message}");
        }

        public class SmsService : IMessageSender
        {
            public void Send(string message) => Console.WriteLine($"SMS sent: {message}");
        }

        // 3. HIGH-LEVEL MODULE
        // DIP SATISFIED: This class depends ONLY on the interface.
        // It has no idea if it's sending an Email, SMS, or Carrier Pigeon.
        public class AlertSystem
        {
            private readonly IMessageSender _messageSender;

            // Dependency is "Injected" through the constructor.
            public AlertSystem(IMessageSender messageSender)
            {
                _messageSender = messageSender;
            }

            public void NotifyUser(string alert)
            {
                _messageSender.Send(alert);
            }
        }
    }
}
    // 4. EXECUTION
// class Program
// {
//     static void Main()
//     {
//         // At runtime, we decide which detail to use.
//         IMessageSender preferredService = new EmailService();

//         // The high-level module is initialized with the chosen detail.
//         AlertSystem app = new AlertSystem(preferredService);
        
//         app.NotifyUser("Your package has arrived!");
//     }
// }
// }

