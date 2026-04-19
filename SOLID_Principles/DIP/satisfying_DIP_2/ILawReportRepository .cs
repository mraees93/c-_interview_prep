using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.DIP.satisfying_DIP_2
{
    public interface ILawReportRepository 
    {
        string GetCaseDetails(int caseId);
    }
}