using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class AsyncLoggingNewParser(ILogger logger, IAsyncNewParser inner)
    : IAsyncNewParser
{
    public async Task<Status<IParser>> Register(
        AsyncAddNewParser add,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронное добавление нового парсера начато.");
        Status<IParser> parser = await inner.Register(add, ct);
        logger.Information("Асинхронное добавление нового парсера закончено.");
        return parser;
    }
}
