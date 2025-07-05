using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink;

public sealed class RemovedParserLink : IRemovedParserLink
{
    public Status<IParserLink> Removed(RemoveParserLink remove)
    {
        IParser parser = remove.TakeOwner();
        LinkFromParserBag link = parser
            .OwnedLinks()
            .FindConcrete(l => new CompareLinkIdentityById(l, remove.RemovingId()));
        return link.Remove()
            ? link.Link().Success()
            : new ParserLinkWithIdNotFoundError(parser, remove.RemovingId());
    }
}
