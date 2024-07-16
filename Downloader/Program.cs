using Downloader.Classes;

using var download = new YoutubeVideoDownloader();
download.Initialize();
await download.ExecuteDownload();