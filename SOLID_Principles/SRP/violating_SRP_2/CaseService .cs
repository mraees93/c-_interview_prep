using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP.violating_SRP_2
{
    public class CaseService
    {
        public void ProcessCase(int id)
        {
            // 1. Business Logic
            Console.WriteLine("Validating case data...");

            // 2. Data Persistence
            Console.WriteLine("Saving to SQL Database...");

            // 3. Reporting
            Console.WriteLine("Generating PDF Report...");
        }
    }
}