using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class AsyncValidatingNewParser(IAsyncNewParser inner) : IAsyncNewParser
{
    public Task<Status<IParser>> Register(AsyncAddNewParser add, CancellationToken ct = default) =>
        new AsyncValidatingOperation(add).Process(inner.Register(add, ct));
}
