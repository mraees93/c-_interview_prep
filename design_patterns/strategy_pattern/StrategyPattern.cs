using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace design_patterns.strategy_pattern
{
    public class StrategyPattern
    {
        // INTERVIEW KEY: The Strategy Abstraction.
        // This interface defines a contract for the behavior that will change at runtime.
        public interface IShippingStrategy
        {
            // Every concrete algorithm must implement this identical signature.
            decimal Calculate(decimal orderTotal);
        }

        // INTERVIEW KEY: Concrete Strategy A.
        // This class encapsulates one specific business rule. It is entirely self-contained.
        public class DHLStrategy : IShippingStrategy
        {
            // Implements DHL-specific logic. If rules change, you only edit this file.
            public decimal Calculate(decimal orderTotal) => orderTotal * 0.15m;
        }

        // INTERVIEW KEY: Concrete Strategy B.
        // Adding a new shipping provider means creating this class without touching DHLStrategy (OCP).
        public class FedExStrategy : IShippingStrategy
        {
            // Implements FedEx-specific calculation rules.
            public decimal Calculate(decimal orderTotal) => orderTotal * 0.10m;
        }

        // INTERVIEW KEY: The Context Class.
        // This class is completely closed for modification. It does not know or care which 
        // concrete strategy it is using; it only communicates via the interface.
        public class OrderProcessor
        {
            // Maintain a private reference to the abstraction layer.
            private readonly IShippingStrategy _shippingStrategy;

            // INTERVIEW CHECK: Dependency Injection.
            // The precise strategy class is resolved and injected at runtime by the .NET DI engine.
            public OrderProcessor(IShippingStrategy shippingStrategy)
            {
                _shippingStrategy = shippingStrategy;
            }

            // This method remains unchanged no matter how many shipping options you add to the system.
            public decimal FinaliseOrder(decimal total)
            {
                // Polymorphic invocation: Calls the specific method of the injected concrete type.
                return total + _shippingStrategy.Calculate(total);
            }
        }

    }
}