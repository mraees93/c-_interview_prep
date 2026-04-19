using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.satisfying_DIP_2
{
    public class LegalSearchService
    {
        private readonly ILawReportRepository _repository;

        // SATISFACTION: Dependency is injected via constructor
        public LegalSearchService(ILawReportRepository repository)
        {
            _repository = repository;
        }

        public void DisplayCase(int id)
        {
            var data = _repository.GetCaseDetails(id);
            Console.WriteLine(data);
        }
    }
}