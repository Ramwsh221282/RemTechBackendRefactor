using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncLoggingDisabledParser(ICustomLogger logger, IAsyncDisabledParser inner)
    : IAsyncDisabledParser
{
    public async Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    )
    {
        logger.Info("Асинхронное выключение парсера.");
        Status<IParser> disabled = await inner.Disable(disable, ct);
        logger.Info("Асинхронное выключение парсера завершено.");
        return disabled;
    }
}
