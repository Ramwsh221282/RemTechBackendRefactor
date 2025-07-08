using RemTech.Logging.Library;

namespace RemTech.Core.Shared.Functional;

public sealed class AsyncLoggingOperation<T>(ICustomLogger logger, string operationName)
{
    public async Task<T> Log(Func<Task<T>> fn)
    {
        logger.Info($"Асинхронное {operationName}. Начато.");
        T result = await fn();
        logger.Info($"Асинхронное {operationName}. Завершено.");
        return result;
    }
}
