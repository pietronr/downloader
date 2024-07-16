using System.Text.RegularExpressions;

namespace Downloader.Helpers;

public static partial class YoutubeHelper
{
    [GeneratedRegex(@"^(https?:\/\/)?(www\.)?(youtube\.com|youtu\.be)\/(watch\?v=|embed\/|v\/|.+\?v=)?([^&=%\?]{11})", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex YouTubeUrlRegex();

    /// <summary>
    /// Verifica se a URL do youtube é válida.
    /// </summary>
    /// <param name="url">URL a ser verificada.</param>
    /// <returns><see langword="true"/> se a URL for válida.</returns>
    public static bool IsValidYouTubeUrl(this string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        return YouTubeUrlRegex().IsMatch(url);
    }
}