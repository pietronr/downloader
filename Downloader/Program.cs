using Downloader.Classes.Youtube;

using var download = new YoutubeVideoDownloader();
download.Initialize();
await download.ExecuteDownload();