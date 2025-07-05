using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Decorators;

public sealed class ValidatingChangedLinkActivity(IChangedLinkActivity inner) : IChangedLinkActivity
{
    public Status<IParserLink> Changed(ChangeLinkActivity change)
    {
        return change.Errored() ? change.Error() : inner.Changed(change);
    }
}
