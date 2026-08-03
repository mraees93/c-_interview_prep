using System;
using System.Threading;
using System.Threading.Tasks;

public class DataSyncService
{
    public async Task SyncRemoteDataAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting data synchronization loop...");

        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine($"Syncing batch {i} of 5...");
            
            await Task.Delay(2000, cancellationToken);

            Console.WriteLine($"Batch {i} successfully written to local disk.");
        }

        Console.WriteLine("Data synchronization complete.");
    }
}
