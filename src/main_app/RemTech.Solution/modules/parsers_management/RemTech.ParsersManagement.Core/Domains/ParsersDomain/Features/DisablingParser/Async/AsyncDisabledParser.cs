using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async;

public sealed class AsyncDisabledParser(IDisabledParser inner) : IAsyncDisabledParser
{
    public Task<Status<IParser>> Disable(AsyncDisableParser disable, CancellationToken ct = default)
    {
        Status<IParser> disabled = inner.Disable(new DisableParser(disable.Take()));
        return Task.FromResult(disabled);
    }
}
