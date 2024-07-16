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

    /// <summary>
    /// Retorna o valor de um object <see cref="Nullable{T}"/>, ou o valor default do objeto.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto.</typeparam>
    /// <param name="value">Valor <see cref="Nullable{T}"/> a ser analisado.</param>
    /// <returns>Valor do objeto, caso exista, ou o valor default.</returns>
    public static T TryValue<T>(this T? value) where T : struct {
        return value.GetValueOrDefault();
    }

    /// <summary>
    /// Retorna o valor de um object <see cref="Nullable{T}"/>, ou o valor padrão definido.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto.</typeparam>
    /// <param name="value">Valor <see cref="Nullable{T}"/> a ser analisado.</param>
    /// <param name="defaultValue">Valor padrão a ser retornado.</param>
    /// <returns>Valor do objeto, caso exista, ou o valor padrão definido.</returns>
    public static T TryValue<T>(this T? value, T defaultValue) where T : struct {
        return value ?? defaultValue;
    }
}
