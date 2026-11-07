using System;
using System.Collections.Generic;

/*
using System;
using System.Collections.Generic;

public class AuditPayload { public string LogId { get; set; } = string.Empty; public string Message { get; set; } = string.Empty; }

public class LocalSplunkCluster
{
    public void StreamLogMetric(string logId, string jsonPayload)
    {
        Console.WriteLine($"[SPLUNK] Log {logId} successfully pushed to corporate logging index.");
    }
}

public class AccessAuditingEngine
{
    private readonly LocalSplunkCluster _splunkCluster;

    public AccessAuditingEngine()
    {
        _splunkCluster = new LocalSplunkCluster();
    }

    public void ForwardLog(AuditPayload payload, string loggingStrategy)
    {
        Console.WriteLine($"Initiating telemetry forwarding pipeline for Record: {payload.LogId}...");
        
        string formattedPayload = string.Empty;

        if (loggingStrategy == "StandardUserActivity")
        {
            formattedPayload = $"[USER_ACTIVITY_LOG]\nData: {payload.Message}";
        }
        else if (loggingStrategy == "AdminOverrideAction")
        {
            formattedPayload = $"[SECURITY_OVERRIDE_ALERT]\nData: {payload.Message}";
        }
        else if (loggingStrategy == "EphemeralGuestTrace")
        {
            throw new NotSupportedException("Ephemeral guest system traces are strictly prohibited from being persisted to central logs!");
        }

        _splunkCluster.StreamLogMetric(payload.LogId, formattedPayload);
    }
}
*/

public class AuditPayload { public string LogId { get; set; } = string.Empty; public string Message { get; set; } = string.Empty; }

public class LocalSplunkCluster : IStreamLogger
{
    public void StreamLogMetric(string logId, string jsonPayload)
    {
        Console.WriteLine($"[SPLUNK] Log {logId} successfully pushed to corporate logging index.");
    }
}
public interface IStreamLogger
{
    void StreamLogMetric(string logId, string jsonPayload);
}
public interface IAuditLogger
{
    string ForwardLog(AuditPayload payload);
}
public class StandardUserActivity : IAuditLogger
{
    public string ForwardLog(AuditPayload payload)
    {
        return $"[USER_ACTIVITY_LOG]\nData: {payload.Message}";
    }
}
public class AdminOverrideAction : IAuditLogger
{
    public string ForwardLog(AuditPayload payload)
    {
        return $"[SECURITY_OVERRIDE_ALERT]\nData: {payload.Message}";
    }
}
public class AccessAuditingEngine
{
    private readonly IStreamLogger _streamLogger;
    public AccessAuditingEngine(IStreamLogger streamLogger)
    {
        _streamLogger = streamLogger;
    }

    public void HandleAuditLog(AuditPayload payload, IAuditLogger auditLogger)
    {
        Console.WriteLine($"Initiating telemetry forwarding pipeline for Record: {payload.LogId}...");
        
        string formattedPayload = auditLogger.ForwardLog(payload);

        _streamLogger.StreamLogMetric(payload.LogId, formattedPayload);
    }
}
