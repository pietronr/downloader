namespace Downloader.Helpers;

public static class TasksHelper
{
    /// <summary>
    /// Executa tasks utilizando semáforos para limitar a concorrência.
    /// Limita em 4 tasks por padrão para CPUs mais simples.
    /// </summary>
    /// <param name="taskFactories">Lista de tasks a serem executadas.</param>
    /// <param name="maxDegreeOfParallelism">Define o máximo de tasks paralelas.</param>
    /// <returns></returns>
    public static async Task ExecuteTasksWithLimitedConcurrency(List<Func<Task>> taskFactories, int maxDegreeOfParallelism = 4)
    {
        using SemaphoreSlim semaphore = new(maxDegreeOfParallelism);
        List<Task> tasks = [];

        foreach (var taskFactory in taskFactories)
        {
            await semaphore.WaitAsync();

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await taskFactory();
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);
    }
}