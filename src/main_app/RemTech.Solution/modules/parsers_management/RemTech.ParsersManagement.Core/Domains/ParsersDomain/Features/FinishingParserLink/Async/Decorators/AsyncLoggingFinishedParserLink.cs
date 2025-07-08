using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async.Decorators;

public sealed class AsyncLoggingFinishedParserLink(
    ICustomLogger logger,
    IAsyncFinishedParserLink inner
) : IAsyncFinishedParserLink
{
    public async Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParserLink>>(
            logger,
            "Остановка парсинга для ссылки."
        ).Log(() => inner.AsyncFinished(finish, ct));
}
