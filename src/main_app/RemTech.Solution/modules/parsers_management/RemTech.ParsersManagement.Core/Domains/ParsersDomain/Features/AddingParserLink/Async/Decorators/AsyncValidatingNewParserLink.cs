using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;

public sealed class AsyncValidatingNewParserLink(IAsyncNewParserLink inner) : IAsyncNewParserLink
{
    public Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default
    )
    {
        bool errored = add.Errored();
        return errored
            ? Task.FromResult(Status<IParserLink>.Failure(add.Error()))
            : inner.AsyncNew(add, ct);
    }
}
