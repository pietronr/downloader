﻿using YoutubeExplode;
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
        Console.WriteLine("\nIniciando download...");

        try
        {
            var video = await _youtubeClient.Videos.GetAsync(midiaUrl);

            var streamInfo = await ExtractStreamInfo(video);

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
        Console.WriteLine("\nSalvando mídia...");

        var (mp4FilePath, mp3FilePath) = SetFilePaths(stream.FileTitle);

        if (!File.Exists(mp4FilePath) || !File.Exists(mp3FilePath))
        {
            var mp4Stream = new FileStream(mp4FilePath, FileMode.Create, FileAccess.Write);
            await stream.Stream.CopyToAsync(mp4Stream);

            await mp4Stream.DisposeAsync();

            if (_shouldSaveOnlyAudio)
            {
                Console.WriteLine($"Convertendo para .mp3: {mp4FilePath}");
                Mp3Helper.ConvertToMp3(mp4FilePath, mp3FilePath, _saveAs320kbps);
            }

            Console.WriteLine($"Arquivo salvo com sucesso!");
        }
        else
        {
            Console.WriteLine($"Arquivo já existente na pasta selecionada!");
        }
    }

    private (string mp4FilePath, string mp3FilePath) SetFilePaths(string fileTitle)
    {
        string defaultPath = Path.Combine(_outputDirectory, $"{fileTitle}.mp4");

        if (_shouldSaveOnlyAudio)
        {
            return (defaultPath, Path.Combine(_outputDirectory, $"{fileTitle}.mp3"));
        }
        else
        {
            return (defaultPath, defaultPath);
        }
    }

    private async Task<IStreamInfo> ExtractStreamInfo(Video video)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

       IStreamInfo streamInfo = streamManifest.GetAudioStreams().OrderByDescending(s => s.Bitrate).ToArray()[0];

        return streamInfo;
    }
}
