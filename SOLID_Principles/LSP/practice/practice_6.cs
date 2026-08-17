using System;

public class Document
{
    public string Content { get; set; } = string.Empty;

    public virtual void Open()
    {
        Console.WriteLine("Opening document and loading text into memory.");
    }

    public virtual void Save(string newContent)
    {
        Content = newContent;
        Console.WriteLine("Saving content changes back to storage.");
    }
}

interface IModifiableDocument
{
    public string Content { get; set; }

    void Save(string newContent);
}

public interface IReadDocument
{
    void Open();
}
public class EditableWordDocument : IReadDocument, IModifiableDocument
{
    public string? Content { get; set; }

    public void Open()
    {
        Console.WriteLine("Opening document and loading text into memory.");
    }
    public void Save(string newContent)
    {
        Content = $"[DOCX Format] {newContent}";
        Console.WriteLine("Successfully updated Word document structure.");
    }
}

public class ReadOnlyPdfDocument : IReadDocument
{
    public void Open()
    {
        Console.WriteLine("Opening document and loading text into memory.");
    }
}

public class TextEditorWorkspace
{
    public void UpdateDocument(IReadDocument document, string updatedText)
    {
        document.Open();
        // OCP COMPLIANCE: By changing the pattern-match type check from the concrete class (EditableWordDocument) to the abstract interface (IModifiableDocument)
        if(document is IModifiableDocument editableDocument) 
        {
            editableDocument.Save(updatedText);
        }
    }
}
