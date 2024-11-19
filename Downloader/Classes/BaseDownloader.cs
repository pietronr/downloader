using Downloader.Helpers;

namespace Downloader.Classes;

public abstract class BaseDownloader : IDisposable
{
    protected readonly HttpClient? _httpClient;

    protected bool _hasInitialized;
    protected bool _saveAs320kbps;
    protected bool _saveAsWav;

    protected const string _folderName = "Pokz_Midias";
    protected string _outputDirectory;
    protected List<string> _urlList;
    protected static readonly char[] separator = [',', ';'];

    protected BaseDownloader(bool initializeClient)
    {
        if (initializeClient) 
            _httpClient = new HttpClient();

        _outputDirectory = string.Empty;
        _urlList = [];
    }

    /// <summary>
    /// Define onde os arquivos serão salvos.
    /// </summary>
    protected void SetOutputDirectoryPath()
    {
        string outputDirectory = ReadUserInput("Digite onde deseja salvar os vídeos ou deixe em branco caso queira salvar na Área de trabalho:");

        while (!Directory.Exists(outputDirectory))
        {
            if (outputDirectory.IsNullOrEmpty())
            {
                string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), _folderName);

                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }

                outputDirectory = appFolder;
                break;
            }
            else
            {
                Console.WriteLine("Caminho inválido");
                outputDirectory = ReadUserInput("Digite novamente o caminho:");
            }
        }

        _outputDirectory = outputDirectory;
    }

    /// <summary>
    /// Define as URLs que devem ser consideradas para download.
    /// </summary>
    private void SetUrls()
    {
        _urlList.Clear();
        string urls = ReadUserInput("\nCole a URL do(s) vídeo(s) que deseja baixar. Se for mais de 1, coloque ',' ou ';' para separá-los.");

        List<string> invalidUrls = [];

        foreach (string url in urls!.Split(separator, StringSplitOptions.TrimEntries))
        {
            if (url.IsValidYouTubeUrl())
            {
                _urlList.Add(url);
            }
            else
            {
                invalidUrls.Add(url);
            }
        }

        if (invalidUrls.Count > 0)
        {
            Console.WriteLine("\nAlgumas URLs são inválidas e não serão consideradas, são elas:");

            foreach (string invalidUrl in invalidUrls)
            {
                Console.WriteLine(invalidUrl);
            }
        }
    }

    /// <summary>
    /// Define como o download será feito.
    /// </summary>
    private void SetDownloadOption()
    {
        string option = ReadUserInput("\nDigite a opção que deseja para salvamento:\n" +
                                       "1 - Áudio simples (.MP3)\n2 - Áudio 320KBPS (.MP3)\n3 - Áudio Wav (.WAV)").Trim();

        while (option != "1" && option != "2" && option != "3")
        {
            Console.WriteLine("Opção inválida");
            option = ReadUserInput("Digite uma opção válida:").Trim();
        }

        int selectedOption = int.Parse(option);

        if (selectedOption == 2) _saveAs320kbps = true;
        else if (selectedOption == 3) _saveAsWav = true;
    }

    /// <summary>
    /// Inicializa e configura a classe para download.
    /// </summary>
    public virtual void Initialize()
    {
        Console.WriteLine("BEM VINDO AO POKZDOWNLOADER\n-------------------------------------------------");

        SetOutputDirectoryPath();

        SetUrls();

        SetDownloadOption();

        _hasInitialized = true;
    }

    /// <summary>
    /// Define se o usuário deseja continuar processando vídeos.
    /// Caso ele finalize inserindo a tecla "N", o programa é encerrado
    /// </summary>
    /// <returns><see langword="true"/> caso o usuário tenha finalizado.</returns>
    public virtual async Task<bool> IsUserDone()
    {
        string continueInput = ReadUserInput("\nDeseja realizar mais downloads? Digite S (SIM) ou N (NÃO).").Trim().ToLower();

        while (continueInput == "s")
        {
            SetUrls();

            await FullDownload();

            continueInput = ReadUserInput("\nDeseja realizar mais downloads? Digite S (SIM) ou N (NÃO).").Trim().ToLower();
        }

        Console.WriteLine("\nObrigado por usar! Pressione qualquer tecla para fechar essa tela.");
        return true;
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

        await FullDownload();

        bool isDone = await IsUserDone();
        return isDone;
    }

    /// <summary>
    /// Realiza o download da lista completa de URLs.
    /// </summary>
    public async Task FullDownload()
    {
        var invalidMidias = await TasksHelper.ExecuteTasksWithLimitedConcurrency(_urlList, DownloadAndSaveMidia);

        Console.WriteLine("-------------------------------------------------\nPROCESSO FINALIZADO!");

        if (!invalidMidias.IsEmpty)
        {
            Console.WriteLine("\nErro no download das seguintes mídias:");

            foreach (var error in invalidMidias)
            {
                Console.WriteLine($"URL: {error.Key}; razão: {error.Value}");
            }
        }
    }

    /// <summary>
    /// Realiza o download e o salvamento da mídia passada via URL.
    /// </summary>
    public virtual async Task DownloadAndSaveMidia(string midiaUrl, CancellationToken token)
    {
        DownloadedFilePath filePath = await DownloadMidia(midiaUrl);

        await ConvertMidia(filePath);

        Console.WriteLine($"Arquivo salvo com sucesso!");
    }

    /// <summary>
    /// Controla a operação de download da mídia.
    /// </summary>
    /// <param name="url">URL para download.</param>
    public abstract Task<DownloadedFilePath> DownloadMidia(string midiaUrl);

    /// <summary>
    /// Controla a operação de conversão da mídia, caso necessário.
    /// </summary>
    public abstract Task ConvertMidia(DownloadedFilePath filePath);

    /// <summary>
    /// Lê a entrada do usuário no console.
    /// </summary>
    /// <param name="message">Mensagem a ser mostrada para o usuário.</param>
    /// <returns>Texto digitado pelo usuário.</returns>
    protected static string ReadUserInput(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine() ?? "";
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _httpClient?.Dispose();
    }
}
