using System;
using System.Threading;
using System.Threading.Tasks;

public class DocumentPayload
{
    public string ReferenceId { get; set; }
    public string RawText { get; set; }
}

public class ComplianceDataController
{
    public async Task ProcessLegalDocumentAsync(DocumentPayload payload)
    {
        Console.WriteLine($"Processing document: {payload.ReferenceId}");

        await Task.Run(() => ValidateStructure(payload.RawText));

        await SavePayloadToClusterAsync(payload);

        Console.WriteLine("Document processing complete.");
    }

    private async Task SavePayloadToClusterAsync(DocumentPayload payload)
    {
        await Task.Delay(1000);
    }

    private bool ValidateStructure(string text)
    {
        return text.Length > 0;
    }
}
