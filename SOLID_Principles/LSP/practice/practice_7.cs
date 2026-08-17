using System;

public class ComplianceDocument : IRawDocument, IMmodifiableDocument
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public void LoadContent(string path)
    {
        Content = $"Raw document data from {path}";
    }

    public void StripMetadata()
    {
        Content = Content.Replace("[META]", "");
        Console.WriteLine("Document structural metadata stripped successfully.");
    }
}

public interface IRawDocument
{
    string Content { get; set; }
    void LoadContent(string path);
}

public interface IMmodifiableDocument
{
    string Content { get; set; }
    public void StripMetadata();
}

public class StandardBrief : IMmodifiableDocument
{
    public string Content { get; set; }
    public void StripMetadata()
    {
        Content = $"[Processed] {Content.Replace("[META]", "")}";
        Console.WriteLine("Standard brief sanitized.");
    }
}

public class SignedStatuteArchive : IRawDocument
{
    public string Content { get; set; }
    public void LoadContent(string path)
    {
        Content = $"[IMMUTABLE CORE VERDICT] Real law text from {path}";
    }
}

public class IngestionPipeline
{
    public void ExecuteIngestion(IRawDocument document, string sourcePath)
    {
        document.LoadContent(sourcePath);
        if (document is IMmodifiableDocument mmodifiable)
        {
            mmodifiable.StripMetadata();
        } 
    }
}
