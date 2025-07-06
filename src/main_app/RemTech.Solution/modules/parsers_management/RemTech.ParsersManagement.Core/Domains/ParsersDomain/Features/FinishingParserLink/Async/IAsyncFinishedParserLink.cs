using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async;

public interface IAsyncFinishedParserLink
{
    Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    );
}