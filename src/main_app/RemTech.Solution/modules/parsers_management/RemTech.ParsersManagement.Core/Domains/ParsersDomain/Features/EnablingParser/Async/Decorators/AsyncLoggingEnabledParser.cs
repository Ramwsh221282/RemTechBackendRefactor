using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class AsyncLoggingEnabledParser(ICustomLogger logger, IAsyncEnabledParser inner)
    : IAsyncEnabledParser
{
    public async Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParser>>(logger, "Включение парсера").Log(
            () => inner.EnableAsync(enable, ct)
        );
}
