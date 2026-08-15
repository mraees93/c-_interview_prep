public class FileStorageService : IFileStorage
{
    public void SaveFileToDisk(string fileName, byte[] content)
    {
        System.IO.File.WriteAllBytes($"C:\\App_Data\\{fileName}", content);
    }
}
public interface IFileStorage
{
    void SaveFileToDisk(string fileName, byte[] content);
}

public class DocumentProcessor
{
    private readonly IFileStorage _storageService;

    public DocumentProcessor(IFileStorage fileStorage)
    {
        _storageService = fileStorage;
    }

    public void UploadDocument(string id, byte[] rawBytes)
    {
        _storageService.SaveFileToDisk($"{id}.pdf", rawBytes);
    }
}
