using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class WorkoutSessionController
{
    private readonly List<string> _activeSessions = new List<string>();

    // Endpoint 1: Start a new live session and alert the system logs
    public async Task StartSession(string clientName)
    {
        _activeSessions.Add(clientName);
        
        // We want to fire off this logging task in the background 
        // without waiting for it to finish before returning to the client
        await LogSessionStartToCloud(clientName); 
    }

    // Endpoint 2: Process metrics for all active clients simultaneously
    public async Task ProcessAllMetrics()
    {
        Console.WriteLine("Processing metrics for all active sessions...");

        List<Task> listOfSessions = new List<Task>();
        foreach (var client in _activeSessions)
        {
            // Goal: Run these calculations concurrently for everyone at once
            listOfSessions.Add(CalculateClientMetricsAsync(client));
        }
        await Task.WhenAll(listOfSessions);

        Console.WriteLine("All metrics calculated successfully.");
    }

    // Endpoint 3: Check if a user's heart rate monitor is responding
    public async Task<bool> IsMonitorRespondingAsync(string clientName)
    {
        // Simulating a fast check that returns a completed task without hitting a database
        var task = Task.FromResult(true);

        // Using await extracts the 'bool' out of the 'Task<bool>'
        return await task;
    }


    // === HELPER METHODS (Assume these work perfectly) ===
    private async Task LogSessionStartToCloud(string clientName)
    {
        await Task.Delay(1000);
        Console.WriteLine($"[Cloud Log] Session started for {clientName}");
    }

    private async Task CalculateClientMetricsAsync(string clientName)
    {
        await Task.Delay(2000); 
        Console.WriteLine($"Metrics compiled for {clientName}");
    }
}
