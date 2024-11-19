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

            var (mp4FilePath, mp3FilePath) = SetFilePaths(video.Title.SanitizeFilePathString());

           if (!File.Exists(mp4FilePath))
           {
                await _streamClient.DownloadAsync(streamInfo, mp4FilePath);

                Console.WriteLine("\nDownload concluído!");
                return new DownloadedFilePath(mp4FilePath, mp3FilePath);
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
        Console.WriteLine($"Convertendo para .mp3: {filePath.Mp4FilePath}");
        Mp3Helper.ConvertToMp3(filePath.Mp4FilePath, filePath.Mp3FilePath, _saveAs320kbps);

        await Task.CompletedTask;
    }

    private (string mp4FilePath, string mp3FilePath) SetFilePaths(string fileTitle)
    {
        string defaultPath = Path.Combine(_outputDirectory, $"{fileTitle}.mp4");

        return (defaultPath, Path.Combine(_outputDirectory, $"{fileTitle}.mp3"));
    }

    private async Task<IStreamInfo> ExtractStreamInfo(Video video)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

        IStreamInfo streamInfo = streamManifest.GetAudioStreams().OrderByDescending(s => s.Bitrate).ToArray()[0];

        return streamInfo;
    }
}
