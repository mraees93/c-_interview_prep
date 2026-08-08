using System;

public class LocalJsonFileWriter : IDataRecorder
{
    public void WriteRecord(string logLine)
    {
        Console.WriteLine($"[DISK WRITE] JSON payload committed: {logLine}");
    }
}

public interface IDataRecorder
{
    void WriteRecord(string logLine);
}

public class BiometricAccessManager 
{
    private readonly IDataRecorder _fileWriter;

    public BiometricAccessManager(IDataRecorder fileWriter)
    {
        _fileWriter = fileWriter;
    }

    public void AuthenticateScan(string employeeId, string scannerId)
    {
        Console.WriteLine($"Processing scan request from node: {scannerId}");
        string logData = $"{DateTime.UtcNow}: Employee {employeeId} passed verification.";
        _fileWriter.WriteRecord(logData);
    }
}
