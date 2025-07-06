using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async;

public interface IAsyncChangedLinkActivity
{
    Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default
    );
}