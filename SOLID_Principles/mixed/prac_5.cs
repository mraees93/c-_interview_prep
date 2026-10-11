/*
using System;
using System.Collections.Generic;

public class CaseFile { public string Id { get; set; } = string.Empty; public string Text { get; set; } = string.Empty; }

public interface ILegalDocumentPipeline
{
    void ExtractPrecedents(CaseFile file);
    void RedactClientNames(CaseFile file);
    void PushToArchiveCluster(CaseFile file);
}

public class HighCourtPipeline : ILegalDocumentPipeline
{
    public string OperatorId { get; set; } = "HC_Bot";
    public List<string> FoundPrecedents { get; set; } = new List<string>();

    public void LogMetrics(string step, string id) => Console.WriteLine($"[METRIC] Operator {OperatorId} ran {step} on Case {id}");

    public void ExtractPrecedents(CaseFile file)
    {
        LogMetrics("Extraction", file.Id);
        FoundPrecedents.Add("S v Zuma");
    }

    public void RedactClientNames(CaseFile file)
    {
        LogMetrics("Redaction", file.Id);
        file.Text = file.Text.Replace("Identity", "[HIDDEN]");
    }

    public void PushToArchiveCluster(CaseFile file) => Console.WriteLine("Archiving securely to disk system.");
}

public class SealedArchivalPipeline : ILegalDocumentPipeline
{
    public string OperatorId { get; set; = "Seal_Bot";
    public List<string> FoundPrecedents { get; set; } = new List<string>();

    public void LogMetrics(string step, string id) => Console.WriteLine($"[METRIC] Operator {OperatorId} ran {step} on Case {id}");

    public void ExtractPrecedents(CaseFile file)
    {
        LogMetrics("Extraction", file.Id);
        FoundPrecedents.Add("Sealed Case");
    }

    public void RedactClientNames(CaseFile file) => throw new InvalidOperationException("Altering sealed historical archives is illegal!");

    public void PushToArchiveCluster(CaseFile file) => Console.WriteLine("Archiving securely to disk system.");
}
*/

using System;
using System.Collections.Generic;

public class CaseFile { public string Id { get; set; } = string.Empty; public string Text { get; set; } = string.Empty; }

public interface IPipelineLogger
{
    void LogMetrics(string step, string id, string operatorId);
}

public interface IPrecedentExtractor { void ExtractPrecedents(CaseFile file); }

public interface IClientRedactor { void RedactClientNames(CaseFile file); }

public abstract class LegalPipelineBase
{
    public string OperatorId { get; set; } = "Base_Bot";
    public List<string> FoundPrecedents { get; set; } = new List<string>();

    public void PushToArchiveCluster(CaseFile file)
    {
        Console.WriteLine("Archiving securely to disk system.");
    }
}

public class HighCourtPipeline : LegalPipelineBase, IPrecedentExtractor, IClientRedactor
{
    private readonly IPipelineLogger _logger;

    public HighCourtPipeline(IPipelineLogger logger)
    {
        _logger = logger;
        OperatorId = "HC_Bot";
    }

    public void ExtractPrecedents(CaseFile file)
    {
        _logger.LogMetrics("Extraction", file.Id, OperatorId);
        FoundPrecedents.Add("S v Zuma");
    }

    public void RedactClientNames(CaseFile file)
    {
        _logger.LogMetrics("Redaction", file.Id, OperatorId);
        file.Text = file.Text.Replace("Identity", "[HIDDEN]");
    }
}

public class SealedArchivalPipeline : LegalPipelineBase, IPrecedentExtractor
{
    private readonly IPipelineLogger _logger;

    public SealedArchivalPipeline(IPipelineLogger logger)
    {
        _logger = logger;
        OperatorId = "Seal_Bot";
    }

    public void ExtractPrecedents(CaseFile file)
    {
        _logger.LogMetrics("Extraction", file.Id, OperatorId);
        FoundPrecedents.Add("Sealed Case");
    }
}


/*
Why this version completely ticks every single box:

• SRP Fixed: Operational log generation is completely offloaded to an injected IPipelineLogger component.
• ISP & LSP Balanced: ILegalDocumentPipeline is completely eliminated. SealedArchivalPipeline only registers for IPrecedentExtractor, making it impossible to trigger a runtime crash.
• Abstract Base Class Utilised: LegalPipelineBase cleanly houses the shared state properties (OperatorId, FoundPrecedents) and implements the shared archival capability (PushToArchiveCluster) exactly once.
*/