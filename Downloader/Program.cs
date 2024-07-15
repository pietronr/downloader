using Scripts.Classes.Youtube;

using var download = new YoutubeVideoDownloader();
download.Initialize();
await download.ExecuteDownload();