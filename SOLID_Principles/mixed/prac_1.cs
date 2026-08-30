// VIOLATION

// using System;

// public class PublicationPayload
// {
//     public string VolumeId { get; set; } = string.Empty;
//     public string RawText { get; set; } = string.Empty;
// }

// public class LocalDiskWriter
// {
//     public void SaveToStorage(string id, string text)
//     {
//         Console.WriteLine($"[DISK WRITE] Volume {id} committed to local storage.");
//     }
// }

// public class PublicationEngine
// {
//     private readonly LocalDiskWriter _diskWriter;

//     public PublicationEngine()
//     {
//         _diskWriter = new LocalDiskWriter();
//     }

//     public void FinalisePublication(PublicationPayload payload, string publicationType)
//     {
//         Console.WriteLine($"Initiating publication sequence for Volume: {payload.VolumeId}...");

//         if (publicationType == "StatuteBook")
//         {
//             payload.RawText = $"[OFFICIAL GOVERNMENT GAZETTE]\n{payload.RawText}";
//         }
//         else if (publicationType == "LawJournal")
//         {
//             payload.RawText = $"[PEER REVIEWED JOURNAL]\n{payload.RawText}";
//         }
//         else if (publicationType == "LiveCourtFeed")
//         {
//             throw new NotSupportedException("Live court feeds cannot be statically formatted or compiled into physical volumes!");
//         }

//         _diskWriter.SaveToStorage(payload.VolumeId, payload.RawText);
//     }
// }


public class PublicationPayload
{
    public string VolumeId { get; set; } = string.Empty;
    public string RawText { get; set; } = string.Empty;
}

public interface ILocalDiskWriter
{
    void SaveToStorage(string id, string text);
}

public abstract class Publication
{
    public abstract void FinalisePublication(PublicationPayload payload);
}

class StatuteBook : Publication
{
    public override void FinalisePublication(PublicationPayload payload)
    {
        payload.RawText = $"[OFFICIAL GOVERNMENT GAZETTE]\n{payload.RawText}";
    }
}

class LawJournal : Publication
{
    public override void FinalisePublication(PublicationPayload payload)
    {
        payload.RawText = $"[PEER REVIEWED JOURNAL]\n{payload.RawText}";
    }
}

public class PublicationEngine
{
    private readonly ILocalDiskWriter _diskWriter;

    public PublicationEngine(ILocalDiskWriter diskWriter)
    {
        _diskWriter = diskWriter;
    }

    public void CompletePublication(PublicationPayload payload, Publication publication)
    {
        Console.WriteLine($"Initiating publication sequence for Volume: {payload.VolumeId}...");

        publication.FinalisePublication(payload);

        _diskWriter.SaveToStorage(payload.VolumeId, payload.RawText);
    }
}
