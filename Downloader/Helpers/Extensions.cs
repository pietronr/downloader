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

    /// <summary>
    /// Verifica se uma string é nula ou vazia.
    /// </summary>
    /// <param name="text">Texto que deve ser verificado.</param>
    /// <returns><see langword="true"/> se a string for nula ou vazia.</returns>
    public static bool IsNullOrEmpty(this string? text)
    {
        return text == null || text == string.Empty;
    }
}
