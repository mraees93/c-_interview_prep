using System;
using System.Collections.Generic;

public class StatuteRecord
{
    public string StatuteId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public interface ISearchSyncPipeline
{
    void IndexStatute(StatuteRecord record);
}

public interface ISearchSyncModifier
{
    void PurgeExpiredStatute(StatuteRecord record);
}
public interface ITelemetryEmitter
{
    void EmitSyncTelemetry(string clusterRegion, string action, string statuteId);
}
public class ConsoleTelemetryEmitter : ITelemetryEmitter
{
    public void EmitSyncTelemetry(string ClusterRegion, string action, string statuteId)
    {
        Console.WriteLine($"[TELEMETRY] {ClusterRegion}: {action} -> {statuteId}");
    }
}
public abstract class ElasticSearch : ISearchSyncPipeline
{
    protected readonly ITelemetryEmitter _telemetryEmitter;
    protected abstract string ClusterRegion { get; }
    protected List<string> SyncedIds = new List<string>();

    protected ElasticSearch(ITelemetryEmitter telemetryEmitter)
    {
        _telemetryEmitter = telemetryEmitter;
    }

    public void IndexStatute(StatuteRecord record)
    {
        _telemetryEmitter.EmitSyncTelemetry(ClusterRegion, "Index", record.StatuteId);
        SyncedIds.Add(record.StatuteId);
    }
}

public class PrimaryElasticSync : ElasticSearch, ISearchSyncModifier
{
    protected override string ClusterRegion => "af-south-1";
    public PrimaryElasticSync(ITelemetryEmitter telemetryEmitter) : base (telemetryEmitter) {}
    
    public void PurgeExpiredStatute(StatuteRecord record)
    {
        _telemetryEmitter.EmitSyncTelemetry(ClusterRegion, "Purge", record.StatuteId);
        SyncedIds.Remove(record.StatuteId);
    }
}

public class ColdArchiveReplicaSync : ElasticSearch
{
    protected override string ClusterRegion => "af-south-1-archive";
    public ColdArchiveReplicaSync(ITelemetryEmitter telemetryEmitter) : base(telemetryEmitter) {}
}
