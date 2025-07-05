using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async;

public sealed class AsyncUpdatedParser(IUpdatedParser inner) : IAsyncUpdatedParser
{
    public Task<Status<IParser>> Update(AsyncUpdateParser update, CancellationToken ct = default)
    {
        IMaybeParser maybeParser = update;
        return Task.FromResult(
            inner.Updated(
                new UpdateParser(maybeParser.Take(), update.MaybeState(), update.MaybeWaitDays())
            )
        );
    }
}
