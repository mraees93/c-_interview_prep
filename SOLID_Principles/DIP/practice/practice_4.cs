using System;

public class ExcelDataWriter : IReportExporter
{
    public void ExportToSpreadsheet(string filename, object data)
    {
        Console.WriteLine($"Writing dataset to file: {filename}.xlsx");
    }
}

public interface IReportExporter
{
    void ExportToSpreadsheet(string filename, object data);
}

public interface IReport
{
    void CompileMonthlyReport(string reportName, object financialData);
}

public class ReportGenerator : IReport
{
    private readonly IReportExporter _excel;

    public ReportGenerator(IReportExporter excel)
    {
        _excel = excel;
    }

    public void CompileMonthlyReport(string reportName, object financialData)
    {
        Console.WriteLine($"Compiling financial data for {reportName}...");
        _excel.ExportToSpreadsheet(reportName, financialData);
    }
}
