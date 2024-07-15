using YoutubeExplode;
using Downloader.Helpers;

namespace Scripts.Classes.Youtube;

public class VideoDownloader : IDisposable
{
    private readonly YoutubeClient _youtubeClient;
    private readonly HttpClient _httpClient;
    private bool _hasInitialized;

    private string _outputDirectory;
    private string[] _urlArray;
    private static readonly char[] separator = [',', ';'];

    public VideoDownloader()
    {
        _youtubeClient = new YoutubeClient();
        _httpClient = new HttpClient();
        _outputDirectory = string.Empty;
        _urlArray = [];
    }

    public void Initialize()
    {
        string? outputDirectory = ReadUserInput("Digite onde deseja salvar os vídeos:");

        while (outputDirectory == null || !Directory.Exists(outputDirectory))
        {
            Console.WriteLine("Caminho inválido");
            outputDirectory = ReadUserInput("Digite novamente o caminho:");
        }

        _outputDirectory = outputDirectory;

        string? urls = ReadUserInput("\nCole a URL do(s) vídeo(s) que deseja baixar. Se for mais de 1, coloque ',' ou ';' para separá-los.\n" +
            "Para colar, copie e presseione o botão direito do mouse.");

        _urlArray = urls!.Split(separator, StringSplitOptions.TrimEntries);

        _hasInitialized = true;
    }

    public async Task<bool> ExecuteDownload()
    {
        if (!_hasInitialized)
        {
            Console.WriteLine("Inicialize a classe para execução do dowload.");
            return false;
        }

        List<Task> tasks = [];

        foreach (string url in _urlArray)
        {
            tasks.Add(DownloadYouTubeVideo(url, _outputDirectory));
        }

        try
        {
            await Task.WhenAll(tasks.AsParallel());
        }
        catch
        {
            Console.WriteLine("Erro no download");
            return false;
        }

        return true;
    }


    private static string? ReadUserInput(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine();
    }

    private async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
    {
        var video = await _youtubeClient.Videos.GetAsync(videoUrl);

        string sanitizedTitle = video.Title.SanitizeFilePathString();

        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);
        var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

        if (muxedStreams.Count != 0)
        {
            var streamInfo = muxedStreams[0];
            var stream = await _httpClient.GetStreamAsync(streamInfo.Url);
            var datetime = DateTime.Now;

            string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
            using var outputStream = File.Create(outputFilePath);
            await stream.CopyToAsync(outputStream);

            Console.WriteLine("\nDownload completed!");
            Console.WriteLine($"Video saved as: {outputFilePath}{datetime}");
        }
        else
        {
            Console.WriteLine($"\nNo suitable video stream found for {video.Title}.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _httpClient.Dispose();
    }
}
