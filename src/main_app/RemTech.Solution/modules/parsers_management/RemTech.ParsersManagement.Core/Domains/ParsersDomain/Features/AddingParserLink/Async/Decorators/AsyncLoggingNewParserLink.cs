using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;

public sealed class AsyncLoggingNewParserLink(ICustomLogger logger, IAsyncNewParserLink inner)
    : IAsyncNewParserLink
{
    public async Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParserLink>>(
            logger,
            "Добавление ссылки парсеру"
        ).Log(() => inner.AsyncNew(add, ct));
}
