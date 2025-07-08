using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncValidatingDisabledParser(IAsyncDisabledParser inner) : IAsyncDisabledParser
{
    public Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    ) => new AsyncValidatingOperation(disable).Process(inner.Disable(disable, ct));
}
