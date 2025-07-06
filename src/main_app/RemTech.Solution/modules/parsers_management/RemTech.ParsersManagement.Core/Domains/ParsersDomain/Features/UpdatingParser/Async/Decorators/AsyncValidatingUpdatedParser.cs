using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;

public sealed class AsyncValidatingUpdatedParser(IAsyncUpdatedParser inner) : IAsyncUpdatedParser
{
    public Task<Status<IParser>> Update(AsyncUpdateParser update, CancellationToken ct = default) =>
        new AsyncValidatingOperation(update).Process(inner.Update(update, ct));
}
