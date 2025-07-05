using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Decorators;

public sealed class ValidatingFinishedParserLink(IFinishedParserLink inner) : IFinishedParserLink
{
    public Status<IParserLink> Finished(FinishParserLink finish)
    {
        return finish.Errored() ? finish.Error() : inner.Finished(finish);
    }
}
