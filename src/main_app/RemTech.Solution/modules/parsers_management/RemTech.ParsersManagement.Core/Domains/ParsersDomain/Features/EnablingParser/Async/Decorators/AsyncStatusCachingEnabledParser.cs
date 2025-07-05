using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class AsyncStatusCachingEnabledParser(IAsyncEnabledParser inner) : IAsyncEnabledParser
{
    private MaybeBag<Status<IParser>> _bag = new();

    public async Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    )
    {
        if (_bag.Any())
            return _bag;
        Status<IParser> parser = await inner.EnableAsync(enable, ct);
        _bag = _bag.Put(parser);
        return parser;
    }
}
