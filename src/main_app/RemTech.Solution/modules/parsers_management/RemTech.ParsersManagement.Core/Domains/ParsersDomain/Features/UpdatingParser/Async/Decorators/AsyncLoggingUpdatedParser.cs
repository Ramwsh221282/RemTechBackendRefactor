using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;

public sealed class AsyncLoggingUpdatedParser(ICustomLogger logger, IAsyncUpdatedParser inner)
    : IAsyncUpdatedParser
{
    public async Task<Status<IParser>> Update(
        AsyncUpdateParser update,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParser>>(
            logger,
            "обновление состояния и(или) дней ожидания парсера"
        ).Log(() => inner.Update(update, ct));
}
