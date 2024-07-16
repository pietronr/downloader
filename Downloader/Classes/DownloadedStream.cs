namespace Downloader.Classes;

public class DownloadedStream(Stream stream, string fileTitle) 
{
    public Stream Stream { get; set; } = stream;

    public string FileTitle { get; set; } = fileTitle;
}