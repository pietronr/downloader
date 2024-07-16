namespace Downloader.Helpers;

public static class TasksHelper
{
    /// <summary>
    /// Executa tasks utilizando <see cref="Parallel"/> para limitar a concorrência.
    /// Limita em 4 tasks por padrão para CPUs mais simples.
    /// </summary>
    /// <param name="source">Lista de items.</param>
    /// <param name="action">Ação assíncrona a ser executada.</param>
    /// <param name="maxDegreeOfParallelism">Define o máximo de tasks paralelas.</param>
    /// <returns></returns>
    public static async Task ExecuteTasksWithLimitedConcurrency<T>(IEnumerable<T> source, Func<T, CancellationToken, Task> action, int maxDegreeOfParallelism = 4)
    {
        await Parallel.ForEachAsync(source, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
            async (item, token) =>
            {
                // logic
                await action(item, token);
            });
    }
}