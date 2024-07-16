using Downloader.Helpers;

namespace Downloader.Classes;

public abstract class BaseDownloader : IDisposable
{
    protected readonly HttpClient _httpClient;

    protected bool _hasInitialized;
    protected bool _shouldSaveAtDesktop;

    protected string _outputDirectory;
    protected string[] _urlArray;
    protected static readonly char[] separator = [',', ';'];

    protected BaseDownloader()
    {
        _httpClient = new HttpClient();
        _outputDirectory = string.Empty;
        _urlArray = [];
    }

    /// <summary>
    /// Inicializa e configura a classe para download.
    /// </summary>
    public virtual void Initialize()
    {
        string? outputDirectory = ReadUserInput("Digite onde deseja salvar os vídeos ou deixe em branco caso queira salvar na Área de trabalho:");

        if (outputDirectory.IsNullOrEmpty())
        {
            _shouldSaveAtDesktop = true;
            outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        else
        {
            while (!Directory.Exists(outputDirectory))
            {
                Console.WriteLine("Caminho inválido");
                outputDirectory = ReadUserInput("Digite novamente o caminho:");
            }
        }

        _outputDirectory = outputDirectory;

        string? urls = ReadUserInput("\nCole a URL do(s) vídeo(s) que deseja baixar. Se for mais de 1, coloque ',' ou ';' para separá-los.\n" +
            "Para colar, copie e pressione o botão direito do mouse.");

        _urlArray = urls!.Split(separator, StringSplitOptions.TrimEntries);

        _hasInitialized = true;
    }

    /// <summary>
    /// Executa o download das mídias informadas via URL.
    /// </summary>
    public virtual async Task<bool> ExecuteDownload()
    {
        if (!_hasInitialized)
        {
            Console.WriteLine("Inicialize a classe para execução do dowload.");
            return false;
        }

        List<Task> tasks = [];

        foreach (string url in _urlArray)
        {
            tasks.Add(DownloadVideo(url));
        }

        try
        {
            await Task.WhenAll(tasks.AsParallel());
        }
        catch
        {
            Console.WriteLine("Erro no download");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Controla a operação de download do vídeo.
    /// </summary>
    /// <param name="url">URL para download.</param>
    public abstract Task DownloadVideo(string videoUrl);

    /// <summary>
    /// Lê a entrada do usuário no console.
    /// </summary>
    /// <param name="message">Mensagem a ser mostrada para o usuário.</param>
    /// <returns>Texto digitado pelo usuário.</returns>
    protected static string? ReadUserInput(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _httpClient.Dispose();
    }
}
