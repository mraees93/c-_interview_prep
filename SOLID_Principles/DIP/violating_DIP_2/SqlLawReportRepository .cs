using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Low-level module: SQL-specific implementation
namespace SOLID_Principles.DIP.violating_DIP_2
{
    public class SqlLawReportRepository 
    {
        public string GetCaseDetails(int caseId) => "Fetching from SQL Server...";
    }
}