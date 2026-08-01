using System;

public class VideoProcessor : ILoadVideo, IVideoFilter, IVideoEncode
{
    public void LoadVideo(string filePath)
    {
        Console.WriteLine($"Loading video file from: {filePath}");
    }

    public void ApplyFilters()
    {
        Console.WriteLine("Applying basic color grading and noise reduction filters.");
    }

    public void EncodeVideo()
    {
        Console.WriteLine("Encoding video stream into H.264 format.");
    }
}

public interface IVideoFilter
{
    void ApplyFilters();
}

public interface IVideoEncode
{
    void EncodeVideo();
}

public interface ILoadVideo
{
    void LoadVideo(string streamUrl);
}

public class Mp4VideoProcessor : IVideoEncode
{
    public void EncodeVideo()
    {
        Console.WriteLine("Encoding video stream into specialized MP4 container.");
    }
}

public class LiveStreamVideoProcessor : ILoadVideo
{
    public void LoadVideo(string streamUrl)
    {
        Console.WriteLine($"Connecting to live RTMP stream at: {streamUrl}");
    }
}

public class ProductionPipeline
{
    public void ProcessMedia(ILoadVideo load, IVideoFilter filter, IVideoEncode encode, string source)
    {
        load.LoadVideo(source);
        filter.ApplyFilters();
        encode.EncodeVideo();
    }
}

