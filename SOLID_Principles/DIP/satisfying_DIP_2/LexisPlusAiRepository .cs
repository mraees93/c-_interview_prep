using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.satisfying_DIP_2
{
    public class LexisPlusAiRepository : ILawReportRepository
    {
        public string GetCaseDetails(int caseId) => "Data from AI-powered research.";
    }
}