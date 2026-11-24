using System;
using System.Collections.Generic;

/*
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
    void PurgeExpiredStatute(StatuteRecord record);
    void EmitSyncTelemetry(string action, string statuteId);
}

public class PrimaryElasticSync : ISearchSyncPipeline
{
    public string ClusterRegion { get; set; } = "af-south-1";
    public List<string> SyncedIds { get; set; } = new List<string>();

    public void EmitSyncTelemetry(string action, string statuteId) => Console.WriteLine($"[TELEMETRY] {ClusterRegion}: {action} -> {statuteId}");

    public void IndexStatute(StatuteRecord record)
    {
        EmitSyncTelemetry("Index", record.StatuteId);
        SyncedIds.Add(record.StatuteId);
    }

    public void PurgeExpiredStatute(StatuteRecord record)
    {
        EmitSyncTelemetry("Purge", record.StatuteId);
        SyncedIds.Remove(record.StatuteId);
    }
}

public class ColdArchiveReplicaSync : ISearchSyncPipeline
{
    public string ClusterRegion { get; set; } = "af-south-1-archive";
    public List<string> SyncedIds { get; set; } = new List<string>();

    public void EmitSyncTelemetry(string action, string statuteId) => Console.WriteLine($"[TELEMETRY] {ClusterRegion}: {action} -> {statuteId}");

    public void IndexStatute(StatuteRecord record)
    {
        EmitSyncTelemetry("Index", record.StatuteId);
        SyncedIds.Add(record.StatuteId);
    }

    public void PurgeExpiredStatute(StatuteRecord record)
    {
        EmitSyncTelemetry("Purge_Ignored", record.StatuteId); // LSP VIOLATION: This child class doesnt do whats prmoised by the parent ISearchSyncPipeline
    }
}
*/

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
    public void EmitSyncTelemetry(string clusterRegion, string action, string statuteId)
    {
        Console.WriteLine($"[TELEMETRY] {clusterRegion}: {action} -> {statuteId}");
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

public class SearchEngine
{
    public void ProcessAndModifyStatute(StatuteRecord record, ElasticSearch searchEngineWorkflow, ISearchSyncModifier syncModifier) // can swap out ElasticSearch for ISearchSyncPipeline if you want a mock object for unit tests
    {
        syncModifier.PurgeExpiredStatute(record);
        searchEngineWorkflow.IndexStatute(record);
    }

    public void ProcessReadOnlyStatute(StatuteRecord record, ElasticSearch searchEngineWorkflow)
    {
        searchEngineWorkflow.IndexStatute(record);
    }
}