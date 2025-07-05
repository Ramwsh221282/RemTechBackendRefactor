using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async;

public sealed class AsyncEnabledParser(IEnabledParser inner) : IAsyncEnabledParser
{
    public Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    ) => Task.FromResult(inner.Enable(enable.Enable()));
}
