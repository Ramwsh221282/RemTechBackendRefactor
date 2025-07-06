using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async.Decorators;

public sealed class AsyncLoggingStoppedParser(ICustomLogger logger, IAsyncStoppedParser inner)
    : IAsyncStoppedParser
{
    public async Task<Status<IParser>> AsyncStopped(
        AsyncStopParser stop,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParser>>(logger, "Остановка парсера.").Log(
            inner.AsyncStopped(stop, ct)
        );
}
