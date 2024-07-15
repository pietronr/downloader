namespace Downloader.Helpers;

public static class Extensions
{
    /// <summary>
    /// Remove caracteres inválidos de arquivos para o salvamento.
    /// </summary>
    /// <param name="text">Texto que deve ser tratado.</param>
    /// <returns>Caminho tratado.</returns>
    public static string SanitizeFilePathString(this string text)
    {
        return string.Join("_", text.Split(Path.GetInvalidFileNameChars()));
    }
}
