using System.Collections.Concurrent;

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
    /// <returns>Um dicionário de items inválidos.</returns>
    public static async Task<ConcurrentDictionary<T, string>> ExecuteTasksWithLimitedConcurrency<T>(IEnumerable<T> source, Func<T, CancellationToken, Task> action, int maxDegreeOfParallelism = 4) where T : class
    {
        ConcurrentDictionary<T, string> invalidItems = [];

        await Parallel.ForEachAsync(source, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
            async (item, token) =>
            {
                try
                {
                    await action(item, token);
                }
                catch (Exception ex)
                {
                    invalidItems.TryAdd(item, ex.Message);
                }
            });

        return invalidItems;
    }
}