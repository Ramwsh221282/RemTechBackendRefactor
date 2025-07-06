using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async.Decorators;

public sealed class AsyncLoggingStartedParser(ICustomLogger logger, IAsyncStartedParser inner)
    : IAsyncStartedParser
{
    public async Task<Status<IParser>> StartedAsync(
        AsyncStartParser start,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParser>>(logger, "Запуск парсера.").Log(
            inner.StartedAsync(start, ct)
        );
}
