using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class StatutePayload
{
    public string Data { get; set; }
}

public class LegalStatuteService
{
    public async Task<List<StatutePayload>> CompileStatuteReportAsync(string actId)
    {
        var primaryTask = FetchFromPrimaryNodeAsync(actId);
        var replicaTask = FetchFromReplicaNodeAsync(actId);

        try
        {
            await Task.WhenAll(primaryTask, replicaTask);
        }
        catch (Exception ex)
        {
            // PANEL TRAP: If BOTH nodes fail and throw different exceptions, 
            // what happens to the secondary exception log entry here?
            Console.WriteLine($"Error recorded during cluster sync: {ex.Message}");
            throw;
        }

        return new List<StatutePayload> { primaryTask.Result, replicaTask.Result };
    }

    private async Task<StatutePayload> FetchFromPrimaryNodeAsync(string id)
    {
        // PANEL TRAP: This library method captures the synchronization context 
        // unnecessarily on every line transition. Optimize it for a shared library.
        await Task.Delay(1000);
        return new StatutePayload { Data = "Primary Act Content" };
    }

    private async Task<StatutePayload> FetchFromReplicaNodeAsync(string id)
    {
        await Task.Delay(1200);
        return new StatutePayload { Data = "Replica Act Content" };
    }
}
