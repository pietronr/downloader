using YoutubeExplode.Videos.Streams;

namespace Downloader.Classes;

public class DownloadedStream(Stream stream, string fileTitle, bool shouldSaveOnlyAudio) 
{
    public Stream Stream { get; set; } = stream;

    public string FileTitle { get; set; } = fileTitle;

    public bool ShouldSaveOnlyAudio { get; set; } = shouldSaveOnlyAudio;
}