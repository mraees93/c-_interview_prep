using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.practice_snippets.practice_1
{
    public class Refactored
    {

        // 1. Define the missing interface (Fixes the 'ISubscriptionRepository' error)
        public interface ISubscriptionRepository
        {
            void UpdateUserStatus(int userId);
        }

        // 2. Define the Abstractions
        public interface ISubscriptionService
        {
            void Activate(int userId);
        }

        public interface IAutoRenewable
        {
            void SetAutoRenewal(bool isActive);
        }

        // 3. Concrete Implementations
        public class PremiumService : ISubscriptionService, IAutoRenewable
        {
            public void Activate(int userId) => Console.WriteLine($"Premium activated for {userId}");
            public void SetAutoRenewal(bool isActive) => Console.WriteLine($"Renewal status: {isActive}");
        }

        public class BasicService : ISubscriptionService
        {
            public void Activate(int userId) => Console.WriteLine($"Basic activated for {userId}");
        }

        // 4. The Repositiory Implementation (To make it runnable)
        public class SubscriptionRepository : ISubscriptionRepository
        {
            public void UpdateUserStatus(int userId) => Console.WriteLine($"DB updated for user {userId}");
        }

        // 5. Clean Manager (Now fully functional)
        public class UserSubscriptionManager
        {
            private readonly ISubscriptionRepository _repo;
            private readonly ISubscriptionService _service;

            // Constructor Injection: All dependencies are passed in from outside
            public UserSubscriptionManager(ISubscriptionRepository repo, ISubscriptionService service)
            {
                _repo = repo;
                _service = service;
            }

            public void ProcessSubscription(int userId)
            {
                _service.Activate(userId);
                _repo.UpdateUserStatus(userId);
            }
        }

    }
}