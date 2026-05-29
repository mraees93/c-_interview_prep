using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//The Decorator pattern is used to add features or responsibilities to an existing object dynamically without modifying its 
//original code or breaking its structure.
namespace design_patterns.decorator_pattern
{
    public class DecoratorPattern
    {
        // INTERVIEW KEY: The Component Interface.
        // This contract defines the behavior that both the core service and its wrappers must share.
        public interface IDataService
        {
            string GetData();
        }

        // INTERVIEW KEY: Core Implementation.
        // This class does EXACTLY one job: handles the pure business/data engine logic (SRP).
        public class SqlDataService : IDataService
        {
            // Core logic only. Do not clutter this class with logging, try-catch, or auditing.
            public string GetData() => "Data from SQL Server";
        }

        // INTERVIEW KEY: The Decorator Wrapper.
        // Crucial: It implements the SAME interface as the target class it is wrapping.
        public class LoggingDataServiceDecorator : IDataService
        {
            // Holds a reference to the inner object being wrapped.
            private readonly IDataService _innerService;

            // INTERVIEW CHECK: Russian Doll composition.
            // It accepts an IDataService. This means it can wrap a raw SqlDataService, 
            // or even wrap another decorator (e.g., CachingDataServiceDecorator wrapping a Logging wrapper).
            public LoggingDataServiceDecorator(IDataService innerService)
            {
                _innerService = innerService;
            }

            // Implementing the interface method allows this class to masquerade as the core service.
            public string GetData()
            {
                // 1. Add Pre-Execution behavior (Cross-cutting concern)
                Console.WriteLine("LOG: Starting data fetch stopwatch...");

                // 2. Delegate the core task to the inner object wrapped inside
                var result = _innerService.GetData();

                // 3. Add Post-Execution behavior
                Console.WriteLine("LOG: Data fetch completed successfully.");

                return result;
            }
        }

    }
}