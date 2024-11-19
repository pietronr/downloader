using YoutubeExplode;
using Downloader.Helpers;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;

namespace Downloader.Classes;

public class YoutubeSongDownloader : BaseDownloader
{
    private readonly YoutubeClient _youtubeClient;
    private readonly StreamClient _streamClient;

    public YoutubeSongDownloader() : base(true)
    {
        _youtubeClient = new YoutubeClient();
        _streamClient = new StreamClient(_httpClient!);
    }

    public override async Task<DownloadedFilePath> DownloadMidia(string midiaUrl)
    {
        Console.WriteLine("\nIniciando download...");

        try
        {
            Video video = await _youtubeClient.Videos.GetAsync(midiaUrl);

            IStreamInfo streamInfo = await ExtractStreamInfo(video);

            var (inputFilePath, outputFilePath) = SetFilePaths(video.Title);

           if (!File.Exists(inputFilePath))
           {
                await _streamClient.DownloadAsync(streamInfo, inputFilePath);

                Console.WriteLine("\nDownload concluído!");
                return new DownloadedFilePath(inputFilePath, outputFilePath);
           }
           else
           {
                Console.WriteLine($"Arquivo já existente na pasta selecionada!");
                throw new InvalidOperationException(); 
           }
        }
        catch (Exception ex) 
        {
            throw new InvalidOperationException($"Erro no download do vídeo. {ex.Message}");
        }
    }

    public override async Task ConvertMidia(DownloadedFilePath filePath)
    {
        Console.WriteLine($"Convertendo arquivo: {filePath.InputFilePath}");
        Mp3Helper.ConvertToMp3OrWav(filePath.InputFilePath, filePath.OutputFilePath, _saveAs320kbps, _saveAsWav);

        await Task.CompletedTask;
    }

    private (string inputFilePath, string outputFilePath) SetFilePaths(string fileTitle)
    {
        fileTitle = fileTitle.SanitizeFilePathString();
        string inputPath = Path.Combine(_outputDirectory, $"{fileTitle}.mp4");

        string outputFilePath;
        if (_saveAsWav)
        {
            outputFilePath = Path.Combine(_outputDirectory, $"{fileTitle}.wav");
        }
        else
        {
            outputFilePath = Path.Combine(_outputDirectory, $"{fileTitle}.mp3");
        }

        return (inputPath, outputFilePath);
    }

    private async Task<IStreamInfo> ExtractStreamInfo(Video video)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

        IStreamInfo streamInfo = streamManifest.GetAudioStreams().OrderByDescending(s => s.Bitrate).ToArray()[0];

        return streamInfo;
    }
}
