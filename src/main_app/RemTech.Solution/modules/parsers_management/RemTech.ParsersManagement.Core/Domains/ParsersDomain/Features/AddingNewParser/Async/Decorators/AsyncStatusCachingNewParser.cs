using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class AsyncStatusCachingNewParser(IAsyncNewParser inner) : IAsyncNewParser
{
    private MaybeBag<Status<IParser>> _bag = new();

    public async Task<Status<IParser>> Register(
        AsyncAddNewParser add,
        CancellationToken ct = default
    )
    {
        if (_bag.Any())
            return _bag.Take();
        Status<IParser> parser = await inner.Register(add, ct);
        _bag = _bag.Put(parser);
        return parser;
    }
}
