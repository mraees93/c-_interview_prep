using System;

/*
using System;

public class CasePayload
{
    public string CaseId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class LocalNetworkDiskWriter
{
    public void SaveToNetworkShare(string path, string text)
    {
        Console.WriteLine($"[IO SHARE] Committed package to network route: {path}");
    }
}

public class CaseDistributionEngine
{
    private readonly LocalNetworkDiskWriter _diskWriter;

    public CaseDistributionEngine()
    {
        _diskWriter = new LocalNetworkDiskWriter();
    }

    public void ForwardCaseFile(CasePayload payload, string allocationModel)
    {
        Console.WriteLine($"Registering transfer envelope for Case: {payload.CaseId}...");

        string finalizedRecord = string.Empty;

        if (allocationModel == "HighCourtRegistry")
        {
            string headers = $"[CLASSIFIED HIGH COURT TRANSCRIPT - RUNNING AT {DateTime.UtcNow}]";
            finalizedRecord = $"{headers}\n{payload.Summary.Trim().ToUpper()}";
        }
        else if (allocationModel == "MagistratesArchive")
        {
            string headers = $"[PUBLIC MAGISTRATES ARCHIVE - RUNNING AT {DateTime.UtcNow}]";
            finalizedRecord = $"{headers}\n{payload.Summary.Trim().ToUpper()}";
        }
        else if (allocationModel == "LiveChamberFeed")
        {
            throw new NotSupportedException("Live chamber feeds are strictly ephemeral data streams; writing to network shares is blocked!");
        }

        string clusterPath = $"//legal-share/verdicts/{payload.CaseId}_Export.txt";
        _diskWriter.SaveToNetworkShare(clusterPath, finalizedRecord);
    }
}
*/

/*
MY APPROACH


public class CasePayload
{
    public string CaseId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class LocalNetworkDiskWriter : INetworkWriter
{
    public void SaveToNetworkShare(string path, string text)
    {
        Console.WriteLine($"[IO SHARE] Committed package to network route: {path}");
    }
}

public abstract class CaseBase
{
    protected static readonly string HighCourtHeader = $"[CLASSIFIED HIGH COURT TRANSCRIPT - RUNNING AT {DateTime.UtcNow}]";
    protected static readonly string MagistratesHeader = $"[PUBLIC MAGISTRATES ARCHIVE - RUNNING AT {DateTime.UtcNow}]";

    protected abstract string Header { get; }
    public string GetFinalizedRecord(CasePayload casePayload)
    {
        return $"{Header}\n{casePayload.Summary.Trim().ToUpper()}";
    } 
}

public interface ICaseHandler
{
    string ForwardCaseFile(CasePayload payload);
}

public class HighCourtRegistry : CaseBase, ICaseHandler
{
    protected override string Header => HighCourtHeader;

    public string ForwardCaseFile(CasePayload payload)
    {
        return GetFinalizedRecord(payload);
    }
}

public class MagistratesArchive : CaseBase, ICaseHandler
{
    protected override string Header => MagistratesHeader;

    public string ForwardCaseFile(CasePayload casePayload)
    {
        return GetFinalizedRecord(casePayload);
    }
}

public interface INetworkWriter
{
    void SaveToNetworkShare(string path, string text);
}
public class CaseDistributionEngine
{
    private readonly INetworkWriter _networkWriter;

    public CaseDistributionEngine(INetworkWriter networkWriter)
    {
        _networkWriter = networkWriter;
    }

    public void HandleCaseFiles(CasePayload payload, ICaseHandler caseHandler)
    {
        Console.WriteLine($"Registering transfer envelope for Case: {payload.CaseId}...");

        string finalizedRecord = caseHandler.ForwardCaseFile(payload);

        string clusterPath = $"//legal-share/verdicts/{payload.CaseId}_Export.txt";
        _networkWriter.SaveToNetworkShare(clusterPath, finalizedRecord);
    }
}
*/

/*
Combine Interface and Base Class (Optional but Cleaner): Right now, you have separate interfaces and abstract classes. 
Since every case distribution type in this template relies on the base class data helper anyway, you can merge ICaseHandler's method 
signature directly into CaseBase. This keeps your class signature line clean and avoids managing two distinct concepts for the same object.
*/

using System;

public class CasePayload
{
    public string CaseId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public interface INetworkWriter
{
    void SaveToNetworkShare(string path, string text);
}

public class LocalNetworkDiskWriter : INetworkWriter
{
    public void SaveToNetworkShare(string path, string text)
    {
        Console.WriteLine($"[IO SHARE] Committed package to network route: {path}");
    }
}

public abstract class CaseBase
{
    protected static readonly string HighCourtHeader = $"[CLASSIFIED HIGH COURT TRANSCRIPT - RUNNING AT {DateTime.UtcNow}]";
    protected static readonly string MagistratesHeader = $"[PUBLIC MAGISTRATES ARCHIVE - RUNNING AT {DateTime.UtcNow}]";

    protected abstract string Header { get; }

    public abstract string ForwardCaseFile(CasePayload payload);

    protected string GetFinalizedRecord(CasePayload casePayload)
    {
        return $"{Header}\n{casePayload.Summary.Trim().ToUpper()}";
    } 
}

public class HighCourtRegistry : CaseBase
{
    protected override string Header => HighCourtHeader;

    public override string ForwardCaseFile(CasePayload payload)
    {
        return GetFinalizedRecord(payload);
    }
}

public class MagistratesArchive : CaseBase
{
    protected override string Header => MagistratesHeader;

    public override string ForwardCaseFile(CasePayload payload)
    {
        return GetFinalizedRecord(payload);
    }
}

public class CaseDistributionEngine
{
    private readonly INetworkWriter _networkWriter;

    public CaseDistributionEngine(INetworkWriter networkWriter)
    {
        _networkWriter = networkWriter;
    }

    public void HandleCaseFiles(CasePayload payload, CaseBase caseHandler)
    {
        Console.WriteLine($"Registering transfer envelope for Case: {payload.CaseId}...");

        string finalizedRecord = caseHandler.ForwardCaseFile(payload);

        string clusterPath = $"//legal-share/verdicts/{payload.CaseId}_Export.txt";
        _networkWriter.SaveToNetworkShare(clusterPath, finalizedRecord);
    }
}
