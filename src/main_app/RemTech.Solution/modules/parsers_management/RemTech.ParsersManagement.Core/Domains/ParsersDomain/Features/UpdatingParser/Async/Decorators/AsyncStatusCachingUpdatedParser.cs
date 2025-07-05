using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;

public sealed class AsyncStatusCachingUpdatedParser(IAsyncUpdatedParser inner) : IAsyncUpdatedParser
{
    private MaybeBag<Status<IParser>> _result = new();

    public async Task<Status<IParser>> Update(
        AsyncUpdateParser update,
        CancellationToken ct = default
    )
    {
        if (_result.Any())
            return _result.Take();
        _result = await inner.Update(update, ct);
        return _result.Take();
    }
}
