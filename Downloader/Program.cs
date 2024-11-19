using Downloader.Classes;

using var download = new YoutubeSongDownloader();
download.Initialize();
await download.ExecuteDownload();

Console.Read();