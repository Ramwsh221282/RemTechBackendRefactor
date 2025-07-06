using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async.Decorators;

public sealed class AsyncValidatingFinishedParserLink(IAsyncFinishedParserLink inner)
    : IAsyncFinishedParserLink
{
    public Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    ) =>
        finish.Errored()
            ? Task.FromResult(Status<IParserLink>.Failure(finish.Error()))
            : inner.AsyncFinished(finish, ct);
}