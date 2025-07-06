using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class AsyncValidatingEnabledParser(IAsyncEnabledParser inner) : IAsyncEnabledParser
{
    public Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    ) => new AsyncValidatingOperation(enable).Process(inner.EnableAsync(enable, ct));
}
