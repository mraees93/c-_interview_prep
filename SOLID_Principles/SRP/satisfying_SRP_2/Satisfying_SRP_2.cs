using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP.satisfying_SRP_2
{
    public class Satisfying_SRP_2
    {
        // 1. Responsible ONLY for persistence
        public class CaseRepository
        {
            public void Save(int id) => Console.WriteLine("Saving to Database...");
        }

        // 2. Responsible ONLY for reporting
        public class ReportGenerator
        {
            public void GeneratePdf(int id) => Console.WriteLine("Generating PDF...");
        }

        // 3. Responsible ONLY for orchestrating business flow
        public class CaseProcessor
        {
            private readonly CaseRepository _repo;
            private readonly ReportGenerator _reporter;

            public CaseProcessor(CaseRepository repo, ReportGenerator reporter)
            {
                _repo = repo;
                _reporter = reporter;
            }

            public void Process(int id)
            {
                // Validation logic here...
                _repo.Save(id);
                _reporter.GeneratePdf(id);
            }
        }

    }
}