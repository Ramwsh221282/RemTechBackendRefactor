using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink;

public sealed class FinishedParserLink : IFinishedParserLink
{
    public Status<IParserLink> Finished(FinishParserLink finish)
    {
        IParser parser = finish.TakeOwner();
        LinkFromParserBag link = parser
            .OwnedLinks()
            .FindConcrete(l => new CompareLinkIdentityById(l, finish.WhoEnded()));
        return link.Any()
            ? parser.Finish(link.Link(), finish.HowMuchTaken())
            : new ParserLinkIsNotFoundInParserError(parser);
    }
}
