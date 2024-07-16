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

    public override async Task<DownloadedStream> DownloadVideo(string videoUrl)
    {
        var video = await _youtubeClient.Videos.GetAsync(videoUrl);

        try
        {
            var (streamInfo, saveOnlyAudio) = await ExtractVideoStream(video);

            string sanitizedTitle = video.Title.SanitizeFilePathString();
            var stream = await _httpClient.GetStreamAsync(streamInfo.Url);

            Console.WriteLine("\nDownload concluído!");
            return new DownloadedStream(stream, sanitizedTitle, saveOnlyAudio);
        }
        catch
        {
            throw new InvalidOperationException("Erro no download do vídeo");
        }
    }

    public override async Task SaveVideo(DownloadedStream stream)
    {
        string outputFilePath = Path.Combine(_outputDirectory, $"{stream.FileTitle}.mp4");
        using var outputStream = File.Create(outputFilePath);
        await stream.Stream.CopyToAsync(outputStream);

        if (stream.ShouldSaveOnlyAudio)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), "tempfile.mp3");
            Mp3Helper.ConvertToMp3(tempFilePath, outputFilePath);
            File.Delete(tempFilePath);
        }

        Console.WriteLine($"Arquivo salvo com sucesso: {outputFilePath}");
    }

    private async Task<(IStreamInfo streamInfo, bool saveOnlyAudio)> ExtractVideoStream(Video video)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

        string? option = ReadUserInput("\nDigite a opção que deseja para salvamento:\n" +
                                       "1 - Vídeo completo (áudio e vídeo)\n2 - Apenas áudio")?.Trim();

        while (option == null || (option != "1" && option != "2"))
        {
            Console.WriteLine("Opção inválida");
            option = ReadUserInput("Digite uma opção válida:");
        }

        int selectedOption = int.Parse(option);
        IStreamInfo streamInfo = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToArray()[0];

        if (selectedOption == 1)
        {
            return (streamInfo, false);
        }
        else
        {
            return (streamInfo, true);
        }
    }
}
