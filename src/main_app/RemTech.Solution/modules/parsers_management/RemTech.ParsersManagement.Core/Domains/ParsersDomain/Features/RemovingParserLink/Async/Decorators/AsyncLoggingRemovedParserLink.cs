using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async.Decorators;

public sealed class AsyncLoggingRemovedParserLink(
    ICustomLogger logger,
    IAsyncRemovedParserLink inner
) : IAsyncRemovedParserLink
{
    public async Task<Status<IParserLink>> AsyncRemoved(
        AsyncRemoveParserLink remove,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParserLink>>(
            logger,
            "Асинхронное удаление ссылки у парсера."
        ).Log(inner.AsyncRemoved(remove, ct));
}
