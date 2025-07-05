using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity;

public class ChangedLinkActivity : IChangedLinkActivity
{
    public Status<IParserLink> Changed(ChangeLinkActivity change)
    {
        IParser parser = change.TakeOwner();
        LinkFromParserBag link = parser
            .OwnedLinks()
            .FindConcrete(l => l.SameBy(new CompareLinkIdentityById(l, change.TakeWhatToChange())));
        return link.Any()
            ? parser.ChangeActivityOf(link.Link(), change.TakeNextActivity())
            : new ParserLinkIsNotFoundInParserError(parser);
    }
}
