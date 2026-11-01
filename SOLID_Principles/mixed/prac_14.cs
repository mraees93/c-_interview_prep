using System;
using System.Collections.Generic;

/*
using System;
using System.Collections.Generic;

public class IndexingPayload { public string DocumentId { get; set; } = string.Empty; public string RawText { get; set; } = string.Empty; }

public class LocalElasticSearchCluster
{
    public void CommitIndexPackage(string docId, string indexData)
    {
        Console.WriteLine($"[ELASTIC] Document {docId} successfully written to search cluster nodes.");
    }
}

public class DocumentIndexingEngine
{
    private readonly LocalElasticSearchCluster _searchCluster;

    public DocumentIndexingEngine()
    {
        _searchCluster = new LocalElasticSearchCluster();
    }

    public void RequestIndexing(IndexingPayload payload, string analysisModel)
    {
        Console.WriteLine($"Spawning background parser worker for Document: {payload.DocumentId}...");
        
        string optimizedContent = string.Empty;

        if (analysisModel == "StandardStatuteText")
        {
            string cleanTokens = payload.RawText.Trim().ToLower();
            optimizedContent = $"[TOKENIZED STATUTE CORES]\nPayload: {cleanTokens}";
        }
        else if (analysisModel == "PublicGazetteText")
        {
            string cleanTokens = payload.RawText.Trim().ToLower();
            optimizedContent = $"[TOKENIZED GAZETTE RECORDS]\nPayload: {cleanTokens}";
        }
        else if (analysisModel == "LiveEncryptedBuffer")
        {
            throw new NotSupportedException("Live encrypted buffers are unverified text streams and are blocked from global search indexing!");
        }

        _searchCluster.CommitIndexPackage(payload.DocumentId, optimizedContent);
    }
}
*/

/* 
MY FIRST TRY - 
Your engine refactoring is great. But look inside your abstract Document class. If we add a third document format type tomorrow, don't we have to modify this 
abstract base class to add another static method? Doesn't that mean your base class violates the Open/Closed Principle?

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

    public abstract string RequestIndexing(IndexingPayload payload);
}
*/

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

public abstract class DocumentBase
{
    protected abstract string Header { get; }

    public string RequestIndexing(IndexingPayload payload)
    {
        string cleanTokens = payload.RawText.Trim().ToLower();
        return $"{Header}\nPayload: {cleanTokens}";
    }
}

public class StandardStatuteDocument : DocumentBase
{
    protected override string Header => "[TOKENIZED STATUTE CORES]";
}

public class PublicGazetteDocument : DocumentBase
{
    protected override string Header => "[TOKENIZED GAZETTE RECORDS]";
}
public class DocumentIndexingEngine
{
    private readonly ISearchEngineHandler _searchEngineHandler;

    public DocumentIndexingEngine(ISearchEngineHandler searchEngineHandler)
    {
        _searchEngineHandler = searchEngineHandler;
    }

    public void RequestDocumentIndexing(IndexingPayload payload, DocumentBase document)
    {
        Console.WriteLine($"Spawning background parser worker for Document: {payload.DocumentId}...");
        
        string optimizedContent = document.RequestIndexing(payload);

        _searchEngineHandler.CommitIndexPackage(payload.DocumentId, optimizedContent);
    }
}
