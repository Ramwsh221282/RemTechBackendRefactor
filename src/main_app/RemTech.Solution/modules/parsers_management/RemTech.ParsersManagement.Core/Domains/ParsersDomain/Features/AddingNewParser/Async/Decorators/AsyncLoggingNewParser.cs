using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class AsyncLoggingNewParser(ICustomLogger logger, IAsyncNewParser inner)
    : IAsyncNewParser
{
    public async Task<Status<IParser>> Register(
        AsyncAddNewParser add,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParser>>(logger, "Добавление нового парсера").Log(
            inner.Register(add, ct)
        );
}
