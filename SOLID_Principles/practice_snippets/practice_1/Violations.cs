using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP
{
    public class Practice
    {
        public class UserSubscriptionManager
        {
            public void ProcessSubscription(int userId, string planType)
            {
                // Section A
                if (planType == "Premium")
                {
                    var service = new PremiumService();
                    service.Activate(userId);
                }
                else if (planType == "Basic")
                {
                    var service = new BasicService();
                    service.Activate(userId);
                }

                // Section B
                var dbConnection = "Server=myServer;Database=UserDB;";
                Console.WriteLine($"Updating database for user {userId} using {dbConnection}");
            }
        }

        public interface ISubscriptionService
        {
            void Activate(int userId);
            void SetAutoRenewal(bool isActive);
        }

        public class PremiumService : ISubscriptionService
        {
            public void Activate(int userId) => Console.WriteLine("Premium Active.");
            public void SetAutoRenewal(bool isActive) => Console.WriteLine("Renewal Set.");
        }

        public class BasicService : ISubscriptionService
        {
            public void Activate(int userId) => Console.WriteLine("Basic Active.");

            public void SetAutoRenewal(bool isActive)
            {
                // Basic plans do not support auto-renewal logic
                throw new InvalidOperationException("Basic accounts cannot set auto-renewal.");
            }
        }

    }
}