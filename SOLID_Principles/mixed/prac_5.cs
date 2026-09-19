using System;
using System.Collections.Generic;

public class ContractDocument
{
    public string CaseId { get; set; } = string.Empty;
    public string RawText { get; set; } = string.Empty;
}

public abstract class Logger
{
    public abstract void LogPipelineMetric(string action, string caseId);
}

public interface IDocumentProcessor
{
    void ExtractClauses(ContractDocument doc);
}

public interface IDocumentEditor{
    void RedactPartyNames(ContractDocument doc);
}

public class ReadOnlyHistoricalBrief : Logger, IDocumentProcessor
{
    public string SystemOperator { get; set; } = "Archival_Bot";
    public List<string> ExtractedTerms { get; set; } = new List<string>();

    public override void LogPipelineMetric(string action, string caseId)
    {
        Console.WriteLine($"[METRIC] Operator {SystemOperator} ran {action} on {caseId}");
    }

    public void ExtractClauses(ContractDocument doc)
    {
        LogPipelineMetric("Extraction", doc.CaseId);
        ExtractedTerms.Add("Historical Precedent");
    }
}

public class EditableHistoricalBrief : Logger, IDocumentEditor
{
    public override void LogPipelineMetric(string action, string caseId)
    {
        Console.WriteLine($"[METRIC] Operator ran {action} on {caseId}");
    }
    public void RedactPartyNames(ContractDocument doc)
    {
        LogPipelineMetric("Redaction", doc.CaseId);
    }
}