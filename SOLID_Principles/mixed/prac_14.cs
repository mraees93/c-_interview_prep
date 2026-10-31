using System;
using System.Collections.Generic;

public class IndexingPayload { public string DocumentId { get; set; } = string.Empty; public string RawText { get; set; } = string.Empty; }

public class LocalElasticSearchCluster : ISearchEngineHandler
{
    public void CommitIndexPackage(string docId, string indexData)
    {
        Console.WriteLine($"[ELASTIC] Document {docId} successfully written to search cluster nodes.");
    }
}

public interface ISearchEngineHandler
{
    void CommitIndexPackage(string docId, string indexData);
}

public abstract class Document
{
    public static string GetCleanTokens(IndexingPayload payload) => payload.RawText.Trim().ToLower();

    public static string GetStandardStatuteText(IndexingPayload payload) {
        string cleanTokens = GetCleanTokens(payload);
        return $"[TOKENIZED STATUTE CORES]\nPayload: {cleanTokens}";
    }
    public static string GetPublicGazetteText(IndexingPayload payload)
    {
        string cleanTokens = GetCleanTokens(payload);
        return $"[TOKENIZED GAZETTE RECORDS]\nPayload: {cleanTokens}";
    }

    public abstract string RequestIndexing();
}

public class StandardStatuteDocument : Document
{
    private readonly IndexingPayload _payload;

    public StandardStatuteDocument(IndexingPayload payload)
    {
        _payload = payload;
    }

    public override string RequestIndexing()
    {
        return GetStandardStatuteText(_payload);
    }
}

public class PublicGazetteDocument : Document
{
    private readonly IndexingPayload _payload;
    public PublicGazetteDocument(IndexingPayload payload)
    {
        _payload = payload;
    }
    public override string RequestIndexing()
    {
        return GetPublicGazetteText(_payload);
    }
}
public class DocumentIndexingEngine
{
    private readonly ISearchEngineHandler _searchEngineHandler;

    public DocumentIndexingEngine(ISearchEngineHandler searchEngineHandler)
    {
        _searchEngineHandler = searchEngineHandler;
    }

    public void RequestDocumentIndexing(IndexingPayload payload, Document document)
    {
        Console.WriteLine($"Spawning background parser worker for Document: {payload.DocumentId}...");
        
        string optimizedContent = document.RequestIndexing();

        _searchEngineHandler.CommitIndexPackage(payload.DocumentId, optimizedContent);
    }
}
