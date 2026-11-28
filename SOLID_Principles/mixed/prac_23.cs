using System;
using System.Threading.Tasks;

/*
using System;
using System.Threading.Tasks;

public class ComplianceDocument
{
    public string DocumentId { get; set; } = string.Empty;
    public string RawContent { get; set; } = string.Empty;
    public int RetentionYears { get; set; } = 7;
}

public class LocalVaultStorage
{
    public async Task CommitArchiveAsync(string archiveKey, string normalizedBody, int years)
    {
        await Task.Delay(50);
        Console.WriteLine($"[VAULT] Stored {archiveKey} for {years} years.");
    }
}

public class ComplianceRetentionEngine
{
    private readonly LocalVaultStorage _vaultStorage;

    public ComplianceRetentionEngine()
    {
        _vaultStorage = new LocalVaultStorage();
    }

    public async Task ArchiveDocumentAsync(ComplianceDocument document, string classification)
    {
        string normalized = document.RawContent.Trim().ToUpper();
        string formattedBody = classification switch
        {
            "TaxAuditRecord" => $"[TAX_COMPLIANCE_ARCHIVE]\n{normalized}",
            "CorporateCharter" => $"[CORPORATE_GOVERNANCE_RECORD]\n{normalized}",
            "TemporaryDraft" => FormatTemporaryDraft(document, normalized),
            _ => throw new ArgumentException("Unknown classification")
        };

        string archiveKey = $"{document.DocumentId}_{classification}.dat";
        await _vaultStorage.CommitArchiveAsync(archiveKey, formattedBody, document.RetentionYears);
    }

    private string FormatTemporaryDraft(ComplianceDocument document, string normalized)
    {
        document.RetentionYears = 0;
        return $"[TEMP_DRAFT_RECORD]\n{normalized}";
    }
}
*/

// public class ComplianceDocument
// {
//     public string DocumentId { get; set; } = string.Empty;
//     public string RawContent { get; set; } = string.Empty;
//     public int RetentionYears { get; set; } = 7;
// }

// public class LocalVaultStorage : IStorageHandler
// {
//     public async Task CommitArchiveAsync(string archiveKey, string normalizedBody, int years)
//     {
//         await Task.Delay(50);
//         Console.WriteLine($"[VAULT] Stored {archiveKey} for {years} years.");
//     }
// }
// public interface IStorageHandler
// {
//     Task CommitArchiveAsync(string archiveKey, string normalizedBody, int years);
// }
// public abstract class DocumentBase
// {
//     protected virtual string GetNormalizedString(ComplianceDocument document) => document.RawContent.Trim().ToUpper();
//     public abstract string Classification { get; }
//     public string FormatBody(ComplianceDocument document)
//     {
//         return $"{Classification}\n{GetNormalizedString(document)}";
//     }
// }

// public class TaxAuditRecord : DocumentBase
// {
//     public override string Classification => "[TAX_COMPLIANCE_ARCHIVE]";
// }

// public class CorporateCharter : DocumentBase
// {
//     public override string Classification => "[CORPORATE_GOVERNANCE_RECORD]";
// }
// public class ComplianceRetentionEngine
// {
//     private readonly IStorageHandler _storageHandler;

//     public ComplianceRetentionEngine(IStorageHandler storageHandler)
//     {
//         _storageHandler = storageHandler;
//     }

//     public async Task ArchiveDocumentAsync(ComplianceDocument document, DocumentBase documentBase)
//     {
//         string formattedBody = documentBase.FormatBody(document);

//         string archiveKey = $"{document.DocumentId}_{documentBase.Classification}.dat";
//         await _storageHandler.CommitArchiveAsync(archiveKey, formattedBody, document.RetentionYears);
//     }
// }
