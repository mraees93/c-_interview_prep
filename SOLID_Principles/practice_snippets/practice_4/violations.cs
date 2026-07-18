using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public record CaseDocument(string Id, string Title, bool IsClassified);

public class CaseDownloadManager
{
    private readonly HttpClient _httpClient;

    public CaseDownloadManager()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://courts.gov") };
    }

    public async Task ProcessDownloadAsync(CaseDocument doc, string storageType)
    {
        if (doc.IsClassified)
        {
            throw new NotSupportedException("Classified documents cannot be processed by this system.");
        }

        Console.WriteLine($"Downloading case: {doc.Title}");
        var response = await _httpClient.GetAsync($"cases/{doc.Id}/pdf");
        var pdfBytes = await response.Content.ReadAsByteArrayAsync();

        if (storageType == "Local")
        {
            await File.WriteAllBytesAsync($"C:\\Storage\\{doc.Id}.pdf", pdfBytes);
        }
        else if (storageType == "S3")
        {
            Console.WriteLine("Uploading to AWS S3 bucket...");
        }
    }
}
