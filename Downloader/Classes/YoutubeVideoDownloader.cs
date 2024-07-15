using YoutubeExplode;
using Downloader.Helpers;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using Downloader.Classes;

namespace Scripts.Classes.Youtube;

public class YoutubeVideoDownloader : BaseDownloader
{
    private readonly YoutubeClient _youtubeClient;

    public YoutubeVideoDownloader() : base()
    {
        _youtubeClient = new YoutubeClient();
    }

    public override async Task DownloadVideo(string videoUrl, string outputDirectory)
    {
        var video = await _youtubeClient.Videos.GetAsync(videoUrl);

        var (streams, saveOnlyAudio) = await ExtractVideoStream(video);

        if (streams.Length > 0)
        {
            string sanitizedTitle = video.Title.SanitizeFilePathString();

            var streamInfo = streams[0];
            var stream = await _httpClient.GetStreamAsync(streamInfo.Url);

            //criar pasta na área de trabalho caso a pessoa escolha essa opção
            //resolver conversão mp3
            Directory.CreateDirectory(outputDirectory);
            string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
            using var outputStream = File.Create(outputFilePath);
            await stream.CopyToAsync(outputStream);

            if (saveOnlyAudio)
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), "tempfile.mp3");
                Mp3Helper.ConvertToMp3(tempFilePath, outputFilePath);
                File.Delete(tempFilePath);
            }

            Console.WriteLine("\nDownload concluído!");
            Console.WriteLine($"Arquivo salvo como: {outputFilePath}");
        }
        else
        {
            Console.WriteLine($"\nNão foram encontrados streams para o vídeo {video.Title}.");
        }
    }

    private async Task<(IStreamInfo[] streams, bool saveOnlyAudio)> ExtractVideoStream(Video video)
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
        IStreamInfo[] streams = [.. streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality)];

        if (selectedOption == 1)
        {
            return (streams, false);
        }
        else
        {
            return (streams, true);
        }
    }
}
