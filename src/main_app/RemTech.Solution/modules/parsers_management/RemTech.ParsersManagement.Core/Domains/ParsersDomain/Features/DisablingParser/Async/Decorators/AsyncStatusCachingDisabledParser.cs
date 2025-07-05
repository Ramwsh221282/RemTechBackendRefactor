using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncStatusCachingDisabledParser(IAsyncDisabledParser inner)
    : IAsyncDisabledParser
{
    private MaybeBag<Status<IParser>> _bag = new();

    public async Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    )
    {
        if (_bag.Any())
            return _bag.Take();
        Status<IParser> disabled = await inner.Disable(disable, ct);
        _bag = _bag.Put(disabled);
        return disabled;
    }
}
