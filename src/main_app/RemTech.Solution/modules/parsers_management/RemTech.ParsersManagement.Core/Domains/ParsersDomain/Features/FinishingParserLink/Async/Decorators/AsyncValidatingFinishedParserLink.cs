using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async.Decorators;

public sealed class AsyncValidatingFinishedParserLink(IAsyncFinishedParserLink inner)
    : IAsyncFinishedParserLink
{
    public Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    ) => new AsyncValidatingOperation(finish).Process(inner.AsyncFinished(finish, ct));
}
