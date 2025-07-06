using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncLoggingDisabledParser(ICustomLogger logger, IAsyncDisabledParser inner)
    : IAsyncDisabledParser
{
    public async Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParser>>(logger, "Выключение парсера").Log(
            inner.Disable(disable, ct)
        );
}
