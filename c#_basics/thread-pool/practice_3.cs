using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// What If both methods throw an exception at the same time?
public class AuditClusterMonitor
{
    public async Task SynchroniseAuditLogsAsync()
    {
        var clusterANode = PushToClusterANodeAsync();
        var clusterBNode = PushToClusterBNodeAsync();

        try
        {
            await Task.WhenAll(clusterANode, clusterBNode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"System Alert: Sync process failed with error -> {ex.Message}");
            throw;
        }
    }

    private async Task PushToClusterANodeAsync()
    {
        await Task.Delay(1000);
        throw new TimeoutException("Cluster A node failed to respond within 1000ms.");
    }

    private async Task PushToClusterBNodeAsync()
    {
        await Task.Delay(1200);
        throw new InvalidOperationException("Cluster B node rejected the request due to a database schema mismatch.");
    }
}
