using YoutubeExplode;
using Downloader.Helpers;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;

namespace Downloader.Classes;

public class YoutubeVideoDownloader : BaseDownloader
{
    private readonly YoutubeClient _youtubeClient;

    public YoutubeVideoDownloader() : base()
    {
        _youtubeClient = new YoutubeClient();
    }

    public override async Task<DownloadedStream> DownloadMidia(string midiaUrl)
    {
        var video = await _youtubeClient.Videos.GetAsync(midiaUrl);

        try
        {
            var streamInfo = await ExtractVideoStream(video);

            string sanitizedTitle = video.Title.SanitizeFilePathString();
            var stream = await _httpClient.GetStreamAsync(streamInfo.Url);

            Console.WriteLine("\nDownload concluído!");
            return new DownloadedStream(stream, sanitizedTitle);
        }
        catch
        {
            throw new InvalidOperationException("Erro no download do vídeo");
        }
    }

    public override async Task SaveMidia(DownloadedStream stream)
    {
        var (outputFilePath, fileStreamPath) = SetFilePaths(stream.FileTitle);

        if (!File.Exists(outputFilePath))
        {
            using var outputStream = new FileStream(fileStreamPath, FileMode.Create, FileAccess.Write);
            await stream.Stream.CopyToAsync(outputStream);

            outputStream.Dispose();

            if (_shouldSaveOnlyAudio)
            {
                Console.WriteLine($"Convertendo para .mp3: {outputFilePath}");
                Mp3Helper.ConvertToMp3(fileStreamPath, outputFilePath);
            }

            Console.WriteLine($"Arquivo salvo com sucesso: {outputFilePath}");
        }
        else
        {
            Console.WriteLine($"Arquivo já existente: {outputFilePath}");
        }
    }

    private (string outputFilePath, string fileStreamPath) SetFilePaths(string fileTitle)
    {
        string defaultPath = Path.Combine(_outputDirectory, $"{fileTitle}.mp4");

        if (_shouldSaveOnlyAudio)
        {
            return (Path.Combine(_outputDirectory, $"{fileTitle}.mp3"), Path.Combine(Path.GetTempPath(), "tempfile.mp3"));
        }
        else
        {
            return (defaultPath, defaultPath);
        }
    }

    private async Task<IStreamInfo> ExtractVideoStream(Video video)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

        IStreamInfo streamInfo = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToArray()[0];

        return streamInfo;
    }
}
