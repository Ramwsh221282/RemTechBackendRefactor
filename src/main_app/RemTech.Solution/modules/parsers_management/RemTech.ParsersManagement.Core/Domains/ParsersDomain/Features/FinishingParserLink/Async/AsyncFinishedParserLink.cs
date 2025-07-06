using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async;

public sealed class AsyncFinishedParserLink(IFinishedParserLink inner) : IAsyncFinishedParserLink
{
    public Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    ) =>
        Task.FromResult(
            inner.Finished(
                new FinishParserLink(finish.Take(), finish.WhomFinishId(), finish.HowMuchTaken())
            )
        );
}