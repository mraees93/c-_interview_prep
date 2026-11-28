using System;
using System.Collections.Generic;

/*
using System;
using System.Collections.Generic;

public class CaseDossier
{
    public string DossierId { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
    public DateTime? ArchivedAtUtc { get; set; }
}

public class LocalAzureBlobVault
{
    public void PersistDossier(string blobName, string payload)
    {
        Console.WriteLine($"[AZURE BLOB] Committed dossier record to cold storage: {blobName}");
    }
}

public class DossierArchivingEngine
{
    private readonly LocalAzureBlobVault _blobVault;

    public DossierArchivingEngine()
    {
        _blobVault = new LocalAzureBlobVault();
    }

    public void ArchiveDossier(CaseDossier dossier, string jurisdictionType)
    {
        string formattedRecord = string.Empty;

        if (jurisdictionType == "HighCourtCivil")
        {
            formattedRecord = $"[HIGH COURT CIVIL ARCHIVE]\n{dossier.SummaryText}";
            dossier.ArchivedAtUtc = DateTime.UtcNow;
        }
        else if (jurisdictionType == "CommercialArbitration")
        {
            formattedRecord = $"[COMMERCIAL ARBITRATION RECORD]\n{dossier.SummaryText}";
            dossier.ArchivedAtUtc = DateTime.UtcNow;
        }
        // LSP violation: Instead of honoring that contract, ActiveAppealPending sneaks out the side door with return;, leaving ArchivedAtUtc as null and skipping 
        // storage completely. when added in a child class along with the other 2 it will change the behavior of what the parent does.
        // How ActiveAppealPending Breaks It: Even though it compiles under the same parent interface, swapping it into ProcessDossier secretly skips persistence 
        // and leaves ArchivedAtUtc as null—breaking the caller's expectations without changing the method signature.
        else if (jurisdictionType == "ActiveAppealPending")
        {
            formattedRecord = $"[ACTIVE APPEAL - HOLD IN STAGING]\n{dossier.SummaryText}";
            dossier.ArchivedAtUtc = null;
            return;
        }

        string blobKey = $"{dossier.DossierId}_sealed.txt";
        _blobVault.PersistDossier(blobKey, formattedRecord);

        if (dossier.ArchivedAtUtc == null)
        {
            throw new InvalidOperationException("Post-condition failed: Archived dossier is missing a valid UTC timestamp!");
        }
    }
}
*/

public class CaseDossier
{
    public string DossierId { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
    public DateTime? ArchivedAtUtc { get; set; }
}

public class LocalAzureBlobVault : IBlobStorageHandler
{
    public void PersistDossier(string blobName, string payload)
    {
        Console.WriteLine($"[AZURE BLOB] Committed dossier record to cold storage: {blobName}");
    }
}

public interface IBlobStorageHandler
{
    void PersistDossier(string blobName, string payload);
}
public interface IDossierProcessor
{
    string ArchiveDossier(CaseDossier dossier);
}
public class HighCourtCivil : IDossierProcessor
{
    public string ArchiveDossier(CaseDossier dossier)
    {
        return $"[HIGH COURT CIVIL ARCHIVE]\n{dossier.SummaryText}";
    }
}
public class CommercialArbitration : IDossierProcessor
{
    public string ArchiveDossier(CaseDossier dossier)
    {
        return $"[COMMERCIAL ARBITRATION RECORD]\n{dossier.SummaryText}";
    }
}
public class DossierArchivingEngine
{
    private readonly IBlobStorageHandler _blobStorageHandler;

    public DossierArchivingEngine(IBlobStorageHandler blobStorageHandler)
    {
        _blobStorageHandler = blobStorageHandler;
    }

    public void ProcessDossier(CaseDossier dossier, IDossierProcessor dossierProcessor)
    {
        string formattedRecord = dossierProcessor.ArchiveDossier(dossier);

        string blobKey = $"{dossier.DossierId}_sealed.txt";
        _blobStorageHandler.PersistDossier(blobKey, formattedRecord);

        dossier.ArchivedAtUtc = DateTime.UtcNow;
    }
}


/*
If you tell the panel: "I separated ActiveAppealPending out of IDossierProcessor so it doesn't silently skip cold storage and break 
the timestamp post-condition; if we still need staging support, we can introduce a dedicated IDossierStagingHandler and ActiveAppealPending 
class," it shows you understand both LSP behavioral contracts and YAGNI (not building extra abstractions until the requirement is confirmed).
*/