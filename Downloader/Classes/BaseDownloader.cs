using Downloader.Helpers;

namespace Downloader.Classes;

public abstract class BaseDownloader : IDisposable
{
    protected readonly HttpClient _httpClient;

    protected bool _hasInitialized;
    protected bool _shouldSaveOnlyAudio;
    protected bool _saveAs320kbps;

    protected const string _folderName = "Pokz_Midias";
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
        Console.WriteLine("BEM VINDO AO POKZDOWNLOADER\n-------------------------------------------------");
        string? outputDirectory = ReadUserInput("Digite onde deseja salvar os vídeos ou deixe em branco caso queira salvar na Área de trabalho:");

        if (outputDirectory.IsNullOrEmpty())
        {
            string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), _folderName);

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            outputDirectory = appFolder;
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

        string? urls = ReadUserInput("\nCole a URL do(s) vídeo(s) que deseja baixar. Se for mais de 1, coloque ',' ou ';' para separá-los.");

        _urlArray = urls!.Split(separator, StringSplitOptions.TrimEntries);

        string? option = ReadUserInput("\nDigite a opção que deseja para salvamento:\n" +
                                       "1 - Vídeo completo (áudio e vídeo)\n2 - Apenas áudio (.MP3)\n3 - Apenas áudio 320KBPS (.MP3)")?.Trim();

        while (option == null || (option != "1" && option != "2" && option != "3"))
        {
            Console.WriteLine("Opção inválida");
            option = ReadUserInput("Digite uma opção válida:");
        }

        int selectedOption = int.Parse(option);

        if (selectedOption == 2 || selectedOption == 3)
        {
            if (selectedOption == 3) _saveAs320kbps = true;
            _shouldSaveOnlyAudio = true;
        }

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
            tasks.Add(DownloadAndSaveMidia(url));
        }

        try
        {
            await Task.WhenAll(tasks);
            Console.WriteLine("-------------------------------------------------\nDownload finalizado!\nPressione qualquer tecla para fechar essa tela.");
        }
        catch
        {
            Console.WriteLine("Erro no download");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Realiza o download e o salvamento da mídia passada via URL.
    /// </summary>
    public virtual async Task DownloadAndSaveMidia(string midiaUrl)
    {
        DownloadedStream stream = await DownloadMidia(midiaUrl);

        await SaveMidia(stream);
    }

    /// <summary>
    /// Controla a operação de download da mídia.
    /// </summary>
    /// <param name="url">URL para download.</param>
    public abstract Task<DownloadedStream> DownloadMidia(string midiaUrl);

    /// <summary>
    /// Controla a operação de salvamento da mídia.
    /// </summary>
    public abstract Task SaveMidia(DownloadedStream stream);

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
