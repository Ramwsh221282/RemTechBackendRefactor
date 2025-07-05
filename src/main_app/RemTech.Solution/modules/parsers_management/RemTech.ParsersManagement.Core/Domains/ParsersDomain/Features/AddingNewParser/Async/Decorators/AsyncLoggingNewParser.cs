using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class AsyncLoggingNewParser(ICustomLogger logger, IAsyncNewParser inner)
    : IAsyncNewParser
{
    public async Task<Status<IParser>> Register(
        AsyncAddNewParser add,
        CancellationToken ct = default
    )
    {
        logger.Info("Асинхронное добавление нового парсера.");
        Status<IParser> parser = await inner.Register(add, ct);
        logger.Info("Асинхронное добавление нового парсера завершено.");
        return parser;
    }
}
