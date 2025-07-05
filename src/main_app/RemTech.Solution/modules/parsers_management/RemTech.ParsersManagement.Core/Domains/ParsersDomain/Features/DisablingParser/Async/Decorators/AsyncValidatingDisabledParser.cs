using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncValidatingDisabledParser(IAsyncDisabledParser inner) : IAsyncDisabledParser
{
    public Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    ) =>
        disable.Errored()
            ? Task.FromResult(Status<IParser>.Failure(disable.Error()))
            : inner.Disable(disable, ct);
}
