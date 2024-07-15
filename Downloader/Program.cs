using Scripts.Classes.Youtube;

using var download = new VideoDownloader();
download.Initialize();
await download.ExecuteDownload();