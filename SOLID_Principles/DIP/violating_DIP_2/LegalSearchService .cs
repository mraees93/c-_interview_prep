using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.violating_DIP_2
{
    public class LegalSearchService 
    {
        // VIOLATION: Hard-coded dependency on a concrete class
        private readonly SqlLawReportRepository _repository = new SqlLawReportRepository();

        public void DisplayCase(int id)
        {
            var data = _repository.GetCaseDetails(id);
            Console.WriteLine(data);
        }
    }
}